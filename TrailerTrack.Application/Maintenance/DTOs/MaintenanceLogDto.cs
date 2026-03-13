using TrailerTrack.Domain.Enums;

namespace TrailerTrack.Application.Maintenance.DTOs;

public record MaintenanceLogDto(
    Guid Id,
    Guid AssetId,
    string Description,
    DateTime CreatedAt,
    DateTime StartedAt,
    DateTime? ExpectedCompletionAt,
    DateTime? CompletedAt,
    string? CompletedNotes,
    string PerformedBy,
    decimal? Cost
);