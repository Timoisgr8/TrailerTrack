using MediatR;
using TrailerTrack.Application.Assets.DTOs;
using TrailerTrack.Application.Maintenance.DTOs;
using TrailerTrack.Application.HireEvents.DTOs;
using TrailerTrack.Application.Common;
using TrailerTrack.Domain.Enums;
using TrailerTrack.Domain.Interfaces;

namespace TrailerTrack.Application.Assets.Queries;

public record GetAssetByIdQuery(Guid Id) : IRequest<Result<AssetDetailDto>>;

public class GetAssetByIdQueryHandler : IRequestHandler<GetAssetByIdQuery, Result<AssetDetailDto>>
{

    private readonly IAssetRepository _assetRepository;
    public GetAssetByIdQueryHandler(IAssetRepository assetRepository)
    {
        _assetRepository = assetRepository;
    }

    public async Task<Result<AssetDetailDto>> Handle(GetAssetByIdQuery request, CancellationToken cancellationToken)
    {
        var asset = await _assetRepository.GetAssetWithDetailsByIdAsync(request.Id, cancellationToken);
        if (asset == null) {
            return Result<AssetDetailDto>.Failure("Asset not found");
        }

        var dto = new AssetDetailDto(
            asset.Id,
            asset.AssetCode,
            asset.Location,
            asset.LastServicedAt,
            asset.CreatedAt,
            asset.UpdatedAt,
            asset.HireEvents.Select(he => new HireEventDto(
                he.Id,
                he.AssetId,
                he.EventType,
                he.EventType.ToLabel(),
                he.EventDate,
                he.CreatedAt,
                he.PerformedBy,
                he.Customer,
                he.CustomerContact,
                he.Notes
            )).ToList(),

            asset.Status,
            asset.Status.ToLabel(),

            asset.Type,
            asset.Type.ToLabel(),

            asset.MaintenanceLogs.Select(ml => new MaintenanceLogDto(
                ml.Id,
                ml.AssetId,
                ml.Description,
                ml.CreatedAt,
                ml.StartedAt,
                ml.ExpectedCompletionAt,
                ml.CompletedAt,
                ml.CompletedNotes,
                ml.PerformedBy,
                ml.Cost
            )).ToList()
            );

        return Result<AssetDetailDto>.Success(dto);
    }
}