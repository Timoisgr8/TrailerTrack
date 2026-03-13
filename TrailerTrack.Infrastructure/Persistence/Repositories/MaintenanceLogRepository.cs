using Microsoft.EntityFrameworkCore;
using TrailerTrack.Domain.Entities;
using TrailerTrack.Domain.Interfaces;

namespace TrailerTrack.Infrastructure.Persistence.Repositories;

public class MaintenanceLogRepository : IMaintenanceLogRepository
{
    private readonly AppDbContext _db;
    public MaintenanceLogRepository(AppDbContext appDbContext)
    {
        _db = appDbContext;
    }

    public async Task AddAsync(MaintenanceLog log, CancellationToken ct = default)
    {
        await _db.AddAsync(log, ct);
    }

    public async Task<MaintenanceLog?> GetActiveByAssetIdAsync(Guid assetId, CancellationToken ct = default)
    {
        return await _db.MaintenanceLogs
            .Where(log => log.AssetId == assetId && log.CompletedAt == null)
            .FirstOrDefaultAsync(ct);
    }

    public async Task<IReadOnlyList<MaintenanceLog>> GetByAssetIdAsync(Guid assetId, CancellationToken ct = default)
    {
        return await _db.MaintenanceLogs
            .Where(log => log.AssetId == assetId)
            .ToListAsync(ct);
    }

    public async Task<MaintenanceLog?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        return await _db.MaintenanceLogs
                    .FirstOrDefaultAsync(log => log.Id == id, ct);
    }

    public async Task SaveChangesAsync(CancellationToken ct = default)
    {
        await _db.SaveChangesAsync(ct);
    }
}
