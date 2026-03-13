using FluentValidation;
using MediatR;
using TrailerTrack.Application.Common;
using TrailerTrack.Domain.Entities;
using TrailerTrack.Domain.Enums;
using TrailerTrack.Domain.Interfaces;

namespace TrailerTrack.Application.HireEvents.Commands;

public record ReturnAssetCommand(
    Guid Id,
    string PerformedBy,
    string Customer,
    DateTime ReturnDate
) : IRequest<Result>;

public class ReturnAssetCommandValidator : AbstractValidator<ReturnAssetCommand>
{
    public ReturnAssetCommandValidator()
    {
        RuleFor(x => x.PerformedBy).NotEmpty().WithMessage("Performed by is required.");
        RuleFor(x => x.Customer).NotEmpty().WithMessage("Customer is required.");
    }
}

public class ReturnAssetCommandHandler : IRequestHandler<ReturnAssetCommand, Result>
{
    private readonly IHireEventRepository _hireEventRepository;
    private readonly IAssetRepository _assetRepository;
    private readonly IValidator<ReturnAssetCommand> _validator;

    public ReturnAssetCommandHandler(IHireEventRepository hireEventRepository, IAssetRepository assetRepository, IValidator<ReturnAssetCommand> validator)
    {
        _hireEventRepository = hireEventRepository;
        _assetRepository = assetRepository;
        _validator = validator;
    }

    public async Task<Result> Handle(ReturnAssetCommand request, CancellationToken cancellationToken)
    {
        var validation = await _validator.ValidateAsync(request, cancellationToken);
        if (!validation.IsValid)
            return Result.Failure(validation.Errors.First().ErrorMessage);

        var asset = await _assetRepository.GetByIdAsync(request.Id, cancellationToken);
        if (asset is null)
            return Result.Failure("Asset not found.");
        if (asset.Status != AssetStatus.HiredOut)
            return Result.Failure("Asset is not currently hired out.");

        asset.UpdateStatus(AssetStatus.Available);

        var returnEvent = HireEvent.CreateReturn(request.Id, request.PerformedBy, request.Customer, request.ReturnDate);

        await _hireEventRepository.AddAsync(returnEvent, cancellationToken);
        await _assetRepository.SaveChangesAsync(cancellationToken); // Note dbcontext saves all repositories when SaveChangesAsync() is called, so we don't need to call it on the hireEventRepository

        return Result.Success();
    }
}

