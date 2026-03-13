using FluentValidation;
using MediatR;
using TrailerTrack.Application.Common;
using TrailerTrack.Application.Interfaces;
using TrailerTrack.Domain.Enums;
using TrailerTrack.Domain.Interfaces;

namespace TrailerTrack.Application.Assets.Commands;

public record RetireAssetCommand(Guid Id) : IRequest<Result>;

public class RetireAssetCommandValidator : AbstractValidator<RetireAssetCommand>
{
    public RetireAssetCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty().WithMessage("Id is required.");
    }
}

public class RetireAssetCommandHandler : IRequestHandler<RetireAssetCommand, Result>
{
    private readonly IAssetRepository _repository;
    private readonly ICurrentUserService _currentUser;
    private readonly IValidator<RetireAssetCommand> _validator;

    public RetireAssetCommandHandler(
        IAssetRepository repository,
        ICurrentUserService currentUser,
        IValidator<RetireAssetCommand> validator
    )
    {
        _repository = repository;
        _currentUser = currentUser;
        _validator = validator;
    }

    public async Task<Result> Handle(RetireAssetCommand request, CancellationToken cancellationToken)
    {
        if (!_currentUser.IsInRole("Admin"))
            return Result.Failure("Unauthorised. Only admins can retire assets.");

        var validation = await _validator.ValidateAsync(request, cancellationToken);
        if (!validation.IsValid)
            return Result.Failure(validation.Errors.First().ErrorMessage);

        var asset = await _repository.GetByIdAsync(request.Id, cancellationToken);
        if (asset is null)
            return Result.Failure("Asset not found.");

        if (asset.Status == AssetStatus.Retired)
            return Result.Failure("Asset is already retired.");

        if (asset.Status == AssetStatus.HiredOut)
            return Result.Failure("Cannot retire an asset that is currently hired out.");

        asset.UpdateStatus(AssetStatus.Retired);
        await _repository.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}