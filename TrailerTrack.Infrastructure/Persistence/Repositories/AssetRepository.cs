using Microsoft.EntityFrameworkCore;
using TrailerTrack.Domain.Entities;
using TrailerTrack.Domain.Enums;
using TrailerTrack.Domain.Interfaces;

namespace TrailerTrack.Infrastructure.Persistence.Repositories;

public class AssetRepository : IAssetRepository
{
    private readonly AppDbContext _db;
    public AssetRepository(AppDbContext appDbContext)
    {
        _db = appDbContext;
    }

    public async Task<Asset?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        return await _db.Assets.FirstOrDefaultAsync(a => a.Id == id, ct);
    }

    public async Task<Asset?> GetAssetWithDetailsByIdAsync(Guid id, CancellationToken ct = default)
    {
        return await _db.Assets
            .Include(a => a.HireEvents)
            .Include(a => a.MaintenanceLogs)
            .FirstOrDefaultAsync(a => a.Id == id, ct);
    }

    public async Task<IReadOnlyList<Asset>> GetAllAsync(CancellationToken ct = default)
    {
        return await _db.Assets.ToListAsync(ct);
    }
    public async Task<IReadOnlyList<Asset>> GetByStatusAsync(AssetStatus status, CancellationToken ct = default)
    {
        return await _db.Assets
            .Where(a => a.Status == status)
            .ToListAsync(ct);
    }

    public async Task<IReadOnlyList<Asset>> GetByTypeAsync(AssetType type, CancellationToken ct = default)
    {
        return await _db.Assets
            .Where(a => a.Type == type)
            .ToListAsync(ct);
    }

    public async Task<bool> ExistsByCodeAsync(string assetCode, CancellationToken ct = default)
    {
        return await _db.Assets.
            AnyAsync(a => a.AssetCode == assetCode, ct);
    }

    public async Task AddAsync(Asset asset, CancellationToken ct = default)
    {
        await _db.Assets.AddAsync(asset, ct);
    }

    public async Task SaveChangesAsync(CancellationToken ct = default)
    {
        await _db.SaveChangesAsync(ct);
    }
}
