using TrailerTrack.Domain.Enums;

namespace TrailerTrack.Domain.Entities;
public class HireEvent
{
    public Guid Id { get; private set; }
    public Guid AssetId { get; private set; }
    public Asset Asset { get; private set; } = null!;
    public HireEventType EventType { get; private set; }
    public DateTime EventDate { get; private set; }

    public DateTime CreatedAt { get; private set; }
    public string PerformedBy { get; private set; } = string.Empty;
    public string Customer { get; private set; } = string.Empty;
    public string CustomerContact { get; private set; } = string.Empty;

    public string? Notes { get; private set; }
    private HireEvent() { } // For EF Core
    public static HireEvent CreateHireOut(Guid assetId, string performedBy, string customer, string customerContact, DateTime hireDate)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(customer);
        ArgumentException.ThrowIfNullOrWhiteSpace(customerContact);
        ArgumentException.ThrowIfNullOrWhiteSpace(performedBy);
        return new HireEvent
        {
            EventType = HireEventType.HiredOut,
            CreatedAt = DateTime.UtcNow,
            Id = Guid.NewGuid(),
            AssetId = assetId,
            PerformedBy = performedBy,
            Customer = customer,
            CustomerContact = customerContact,
            EventDate = hireDate
        };
    }
    
    public static HireEvent CreateReturn(Guid assetId, string performedBy, string customer, DateTime returnDate)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(customer);
        ArgumentException.ThrowIfNullOrWhiteSpace(performedBy);
        return new HireEvent
        {
            EventType = HireEventType.Returned,
            CreatedAt = DateTime.UtcNow,
            Id = Guid.NewGuid(),
            AssetId = assetId,
            PerformedBy = performedBy,
            Customer = customer,
            EventDate = returnDate
        };
    }
}