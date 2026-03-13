using FluentValidation;
using MediatR;
using TrailerTrack.Application.Assets.Commands;
using TrailerTrack.Application.Common;
using TrailerTrack.Application.Interfaces;
using TrailerTrack.Domain.Entities;
using TrailerTrack.Domain.Enums;
using TrailerTrack.Domain.Interfaces;

namespace TrailerTrack.Application.Maintenance.Commands;

public record CompleteMaintenanceCommand(
    string CompletedNotes,
    string PerformedBy,
    decimal? Cost,
    Guid Id
) : IRequest<Result>;

public class CompleteMaintenanceCommandValidator : AbstractValidator<CompleteMaintenanceCommand>
{
    public CompleteMaintenanceCommandValidator()
    {
        RuleFor(x => x.CompletedNotes).NotEmpty().WithMessage("Completed notes are required.");
        RuleFor(x => x.PerformedBy).NotEmpty().WithMessage("Performed by is required.");
        RuleFor(x => x.Cost).GreaterThanOrEqualTo(0).When(x => x.Cost.HasValue).WithMessage("Cost must be non negative.");
    }
}

public class CompleteMaintenanceCommandHandler : IRequestHandler<CompleteMaintenanceCommand, Result>
{
    private readonly IMaintenanceLogRepository _maintenanceLogRepository;
    private readonly IAssetRepository _assetRepository;
    private readonly ICurrentUserService _currentUser;
    private readonly IValidator<CompleteMaintenanceCommand> _validator;

    public CompleteMaintenanceCommandHandler(
        IMaintenanceLogRepository maintenanceLogRepository,
        IAssetRepository assetRepository,
        ICurrentUserService currentUser,
        IValidator<CompleteMaintenanceCommand> validator
    )
    {
        _maintenanceLogRepository = maintenanceLogRepository;
        _assetRepository = assetRepository;
        _currentUser = currentUser;
        _validator = validator;
    }

    public async Task<Result> Handle(CompleteMaintenanceCommand request, CancellationToken cancellationToken)
    {
        if (!_currentUser.IsInRole("Admin"))
            return Result.Failure("Unauthorised. Only admins can retire assets.");

        var validation = await _validator.ValidateAsync(request, cancellationToken);
        if (!validation.IsValid)
            return Result.Failure(validation.Errors.First().ErrorMessage);


        var maintenanceLog = await _maintenanceLogRepository.GetByIdAsync(request.Id, cancellationToken);
        if (maintenanceLog is null)
            return Result.Failure("Maintenance Log not found.");
        if (maintenanceLog.CompletedAt != null)
            return Result.Failure("Maintenance Log is already completed.");

        var asset = await _assetRepository.GetByIdAsync(maintenanceLog.AssetId, cancellationToken);
        if (asset is null)
            return Result.Failure("Asset not found.");

        asset.UpdateStatus(AssetStatus.Available);
        asset.UpdateLastServicedAt();


        maintenanceLog.CompleteMaintenance(request.CompletedNotes, request.PerformedBy, request.Cost);

        await _assetRepository.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}

