using FluentValidation;
using MediatR;
using TrailerTrack.Application.Common;
using TrailerTrack.Application.Interfaces;
using TrailerTrack.Domain.Entities;
using TrailerTrack.Domain.Enums;
using TrailerTrack.Domain.Interfaces;

namespace TrailerTrack.Application.Maintenance.Commands;

public record StartMaintenanceCommand(
    string Description,
    Guid AssetId,
    DateTime StartedAt,
    DateTime? ExpectedCompletionAt
) : IRequest<Result>;

public class StartMaintenanceCommandValidator : AbstractValidator<StartMaintenanceCommand>
{
    public StartMaintenanceCommandValidator()
    {
        RuleFor(x => x.Description).NotEmpty().WithMessage("Description is required.");
        RuleFor(x => x.ExpectedCompletionAt).GreaterThan(x => x.StartedAt).When(x => x.ExpectedCompletionAt.HasValue).WithMessage("Expected completion date must be after the start date.");
    }
}

public class StartMaintenanceCommandHandler : IRequestHandler<StartMaintenanceCommand, Result>
{
    private readonly IMaintenanceLogRepository _maintenanceLogRepository;
    private readonly IAssetRepository _assetRepository;
    private readonly ICurrentUserService _currentUser;
    private readonly IValidator<StartMaintenanceCommand> _validator;

    public StartMaintenanceCommandHandler(
        IMaintenanceLogRepository maintenanceLogRepository,
        IAssetRepository assetRepository,
        ICurrentUserService currentUser,
        IValidator<StartMaintenanceCommand> validator
        )
    {
        _maintenanceLogRepository = maintenanceLogRepository;
        _assetRepository = assetRepository;
        _currentUser = currentUser;
        _validator = validator;
    }

    public async Task<Result> Handle(StartMaintenanceCommand request, CancellationToken cancellationToken)
    {
        if (!_currentUser.IsInRole("Admin"))
            return Result.Failure("Unauthorised. Only admins can retire assets.");

        var validation = await _validator.ValidateAsync(request, cancellationToken);
        if (!validation.IsValid)
            return Result.Failure(validation.Errors.First().ErrorMessage);

        var asset = await _assetRepository.GetByIdAsync(request.AssetId, cancellationToken);
        if (asset is null)
            return Result.Failure("Asset not found.");
        if (asset.Status == AssetStatus.Retired)
            return Result.Failure("Cannot log maintenance on a retired asset.");
        if (asset.Status == AssetStatus.HiredOut)
            return Result.Failure("Cannot start maintenance on a hired out asset.");
        if (asset.Status == AssetStatus.UnderMaintenance)
            return Result.Failure("Asset is already under maintenance.");

        asset.UpdateStatus(AssetStatus.UnderMaintenance);

        var maintenanceLog = MaintenanceLog.StartMaintenance(request.Description, request.AssetId, request.StartedAt, request.ExpectedCompletionAt);
        await _maintenanceLogRepository.AddAsync(maintenanceLog, cancellationToken);
        await _assetRepository.SaveChangesAsync(cancellationToken); // Note dbcontext saves all repositories when SaveChangesAsync() is called, so we don't need to call it on the hireEventRepository

        return Result.Success();
    }
}

