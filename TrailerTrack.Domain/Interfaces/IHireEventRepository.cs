using TrailerTrack.Domain.Entities;
using TrailerTrack.Domain.Enums;

namespace TrailerTrack.Domain.Interfaces;

public interface IHireEventRepository
{
    Task<HireEvent?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<IReadOnlyList<HireEvent>> GetByAssetIdAsync(Guid id, CancellationToken ct = default);
    Task AddAsync(HireEvent hireEvent, CancellationToken ct = default);
    Task SaveChangesAsync(CancellationToken ct = default);
}
