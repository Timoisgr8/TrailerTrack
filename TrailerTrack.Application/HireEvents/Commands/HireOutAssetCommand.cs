using FluentValidation;
using MediatR;
using TrailerTrack.Application.Common;
using TrailerTrack.Domain.Entities;
using TrailerTrack.Domain.Enums;
using TrailerTrack.Domain.Interfaces;

namespace TrailerTrack.Application.HireEvents.Commands;

public record HireOutAssetCommand(
    Guid Id,
    string PerformedBy,
    string Customer,
    string CustomerContact,
    DateTime HireDate
) : IRequest<Result>;

public class HireOutAssetCommandValidator : AbstractValidator<HireOutAssetCommand>
{
    public HireOutAssetCommandValidator()
    {
        RuleFor(x => x.PerformedBy).NotEmpty().WithMessage("Performed by is required.");
        RuleFor(x => x.Customer).NotEmpty().WithMessage("Customer is required.");
        RuleFor(x => x.CustomerContact).NotEmpty().WithMessage("Customer contact is required.");
    }
}

public class HireOutAssetCommandHandler : IRequestHandler<HireOutAssetCommand, Result>
{
    private readonly IHireEventRepository _hireEventRepository;
    private readonly IAssetRepository _assetRepository;
    private readonly IValidator<HireOutAssetCommand> _validator;

    public HireOutAssetCommandHandler(IHireEventRepository hireEventRepository, IAssetRepository assetRepository, IValidator<HireOutAssetCommand> validator)
    {
        _hireEventRepository = hireEventRepository;
        _assetRepository = assetRepository;
        _validator = validator;
    }

    public async Task<Result> Handle(HireOutAssetCommand request, CancellationToken cancellationToken)
    {
        var validation = await _validator.ValidateAsync(request, cancellationToken);
        if (!validation.IsValid)
            return Result.Failure(validation.Errors.First().ErrorMessage);

        var asset = await _assetRepository.GetByIdAsync(request.Id, cancellationToken);
        if (asset is null)
            return Result.Failure("Asset not found.");
        if (asset.Status != AssetStatus.Available)
            return Result.Failure("Asset is not available for hire.");

        asset.UpdateStatus(AssetStatus.HiredOut);

        var hireOutEvent = HireEvent.CreateHireOut(request.Id, request.PerformedBy, request.Customer, request.CustomerContact, request.HireDate);
        await _hireEventRepository.AddAsync(hireOutEvent, cancellationToken);
        await _assetRepository.SaveChangesAsync(cancellationToken); // Note dbcontext saves all repositories when SaveChangesAsync() is called, so we don't need to call it on the hireEventRepository

        return Result.Success();
    }
}

