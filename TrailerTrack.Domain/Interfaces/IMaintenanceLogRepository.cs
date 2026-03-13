using TrailerTrack.Domain.Entities;
using TrailerTrack.Domain.Enums;

namespace TrailerTrack.Domain.Interfaces;

public interface IMaintenanceLogRepository
{
    Task<MaintenanceLog?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<MaintenanceLog?> GetActiveByAssetIdAsync(Guid assetId, CancellationToken ct = default);
    Task<IReadOnlyList<MaintenanceLog>> GetByAssetIdAsync(Guid assetId, CancellationToken ct = default);
    Task AddAsync(MaintenanceLog log, CancellationToken ct = default);
    Task SaveChangesAsync(CancellationToken ct = default);
}
