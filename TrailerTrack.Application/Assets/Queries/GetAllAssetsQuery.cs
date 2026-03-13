using MediatR;
using TrailerTrack.Application.Assets.DTOs;
using TrailerTrack.Application.Common;
using TrailerTrack.Domain.Enums;
using TrailerTrack.Domain.Interfaces;

namespace TrailerTrack.Application.Assets.Queries;

public record GetAllAssetsQuery(AssetStatus? Status = null) : IRequest<Result<IReadOnlyList<AssetDto>>>;

public class GetAllAssetsQueryHandler : IRequestHandler<GetAllAssetsQuery, Result<IReadOnlyList<AssetDto>>>
{

    private readonly IAssetRepository _assetRepository;
    public GetAllAssetsQueryHandler(IAssetRepository assetRepository)
    {
        _assetRepository = assetRepository;
    }

    public async Task<Result<IReadOnlyList<AssetDto>>> Handle(GetAllAssetsQuery request, CancellationToken cancellationToken)
    {
        var assets = request.Status.HasValue
            ? await _assetRepository.GetByStatusAsync(request.Status.Value, cancellationToken)
            : await _assetRepository.GetAllAsync(cancellationToken);
        var dto = assets.Select(a => new AssetDto(
            a.Id,
            a.AssetCode,
            a.Location,
            a.Status,
            a.Status.ToLabel(),
            a.Type,
            a.Type.ToLabel(),
            a.LastServicedAt
        )).ToList();

        return Result<IReadOnlyList<AssetDto>>.Success(dto);
    }
}