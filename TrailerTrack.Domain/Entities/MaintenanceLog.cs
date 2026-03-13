namespace TrailerTrack.Domain.Entities;

public class MaintenanceLog
{
    public string Description { get; private set; } = string.Empty;
    public Guid Id { get; private set; }
    public Guid AssetId { get; private set; }
    public Asset Asset { get; private set; } = null!;
    public DateTime CreatedAt { get; private set; }
    public DateTime StartedAt { get; private set; }
    public DateTime? ExpectedCompletionAt { get; private set; }
    public DateTime? CompletedAt { get; private set; }
    public string? CompletedNotes { get; private set; }
    public string PerformedBy { get; private set; } = string.Empty;
    public decimal? Cost { get; private set; }

    private MaintenanceLog() { }  // For EF Core

    public static MaintenanceLog StartMaintenance(string description, Guid assetId, DateTime startedAt, DateTime? expectedCompletionAt)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(description);
        return new MaintenanceLog
        {
            Description = description,
            Id = Guid.NewGuid(),
            AssetId = assetId,
            CreatedAt = DateTime.UtcNow,
            StartedAt = startedAt,
            ExpectedCompletionAt = expectedCompletionAt,
            CompletedAt = null,
            CompletedNotes = null,
        };
    }



    public void CompleteMaintenance(string completedNotes, string performedBy, decimal? cost)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(performedBy);
        ArgumentException.ThrowIfNullOrWhiteSpace(completedNotes);


        CompletedAt = DateTime.UtcNow;
        CompletedNotes = completedNotes;
        PerformedBy = performedBy;
        Cost = cost;
    }

}