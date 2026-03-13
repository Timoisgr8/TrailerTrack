using TrailerTrack.Domain.Entities;
using TrailerTrack.Domain.Enums;

namespace TrailerTrack.Domain.Interfaces;

public interface IAssetRepository
{
    Task<Asset?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<Asset?> GetAssetWithDetailsByIdAsync(Guid id, CancellationToken ct = default);
    Task<IReadOnlyList<Asset>> GetAllAsync(CancellationToken ct = default);
    Task<IReadOnlyList<Asset>> GetByStatusAsync(AssetStatus status, CancellationToken ct = default);
    Task<IReadOnlyList<Asset>> GetByTypeAsync(AssetType type, CancellationToken ct = default);
    Task<bool> ExistsByCodeAsync(string assetCode, CancellationToken ct = default);
    Task AddAsync(Asset asset, CancellationToken ct = default);
    Task SaveChangesAsync(CancellationToken ct = default);
}
