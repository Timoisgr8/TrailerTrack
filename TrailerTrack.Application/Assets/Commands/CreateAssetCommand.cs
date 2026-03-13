using FluentValidation;
using MediatR;
using TrailerTrack.Application.Common;
using TrailerTrack.Domain.Entities;
using TrailerTrack.Domain.Enums;
using TrailerTrack.Domain.Interfaces;

namespace TrailerTrack.Application.Assets.Commands;

public record CreateAssetCommand(
    string AssetCode,
    string Location,
    AssetType AssetType
) : IRequest<Result<Guid>>;

public class CreateAssetCommandValidator : AbstractValidator<CreateAssetCommand>
{
    public CreateAssetCommandValidator()
    {
        RuleFor(x => x.AssetCode).NotEmpty().WithMessage("Asset code is required.");
        RuleFor(x => x.Location).NotEmpty().WithMessage("Location is required.");
    }
}

public class CreateAssetCommandHandler : IRequestHandler<CreateAssetCommand, Result<Guid>>
{
    private readonly IAssetRepository _assetRepository;
    private readonly IValidator<CreateAssetCommand> _validator;
    public CreateAssetCommandHandler(IAssetRepository assetRepository, IValidator<CreateAssetCommand> validator)
    {
        _assetRepository = assetRepository;
        _validator = validator;
    }
    public async Task<Result<Guid>> Handle(CreateAssetCommand request, CancellationToken cancellationToken)
    {
        var validation = await _validator.ValidateAsync(request, cancellationToken);
        if (!validation.IsValid)
            return Result<Guid>.Failure(validation.Errors.First().ErrorMessage);

        if (await _assetRepository.ExistsByCodeAsync(request.AssetCode, cancellationToken))
            return Result<Guid>.Failure("Asset with this code already exists.");

        var asset = Asset.Create(request.AssetCode, request.Location, request.AssetType);
        await _assetRepository.AddAsync(asset, cancellationToken);
        await _assetRepository.SaveChangesAsync(cancellationToken);

        return Result<Guid>.Success(asset.Id);
    }
}

