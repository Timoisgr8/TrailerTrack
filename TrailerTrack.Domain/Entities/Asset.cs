using TrailerTrack.Domain.Enums;

namespace TrailerTrack.Domain.Entities;

public class Asset
{
    public Guid Id { get; private set; }
    public string AssetCode { get; private set; } = string.Empty;
    public string Location { get; private set; } = string.Empty;
    public DateTime? LastServicedAt { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    private readonly List<HireEvent> _hireEvents = new();
    public IReadOnlyCollection<HireEvent> HireEvents => _hireEvents.AsReadOnly();

    public AssetType Type { get; private set; }
    public AssetStatus Status { get; private set; }

    private readonly List<MaintenanceLog> _maintenanceLogs = new();
    public IReadOnlyCollection<MaintenanceLog> MaintenanceLogs => _maintenanceLogs.AsReadOnly();
    private Asset () { } // For EF Core

    public static Asset Create(string assetCode, string location, AssetType type)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(assetCode);
        ArgumentException.ThrowIfNullOrWhiteSpace(location);

        return new Asset
        {
            Id = Guid.NewGuid(),
            AssetCode = assetCode,
            Location = location,
            Type = type,
            Status = AssetStatus.Available,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
    }

    public void UpdateLocation(string newLocation)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(newLocation);
        Location = newLocation;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateStatus(AssetStatus newStatus)
    {
        Status = newStatus;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateLastServicedAt()
    {
        LastServicedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }
}