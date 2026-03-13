using Microsoft.EntityFrameworkCore;
using TrailerTrack.Domain.Entities;
using TrailerTrack.Domain.Enums;
using TrailerTrack.Domain.Interfaces;

namespace TrailerTrack.Infrastructure.Persistence.Repositories;

public class HireEventRepository : IHireEventRepository
{
    private readonly AppDbContext _db;
    public HireEventRepository(AppDbContext appDbContext)
    {
        _db = appDbContext;
    }

    public async Task AddAsync(HireEvent hireEvent, CancellationToken ct = default)
    {
        await _db.AddAsync(hireEvent, ct);
    }

    public async Task<IReadOnlyList<HireEvent>> GetByAssetIdAsync(Guid id, CancellationToken ct = default)
    {
        return await _db.HireEvents
            .Where(h => h.AssetId == id)
            .ToListAsync(ct);
    }

    public async Task<HireEvent?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        return await _db.HireEvents.FirstOrDefaultAsync(a => a.Id == id, ct);
    }

    public async Task SaveChangesAsync(CancellationToken ct = default)
    {
        await _db.SaveChangesAsync(ct);
    }
}
