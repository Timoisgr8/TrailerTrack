using TrailerTrack.Domain.Enums;
using TrailerTrack.Application.HireEvents.DTOs;
using TrailerTrack.Application.Maintenance.DTOs;

namespace TrailerTrack.Application.Assets.DTOs;

public record AssetDetailDto(
    Guid Id,
    
    string AssetCode,
    
    string Location,

    DateTime? LastServicedAt,

    DateTime CreatedAt,

    DateTime UpdatedAt,

    IReadOnlyCollection<HireEventDto> HireEvents,

    AssetStatus Status,
    string StatusLabel,

    AssetType Type,
    string TypeLabel,

    IReadOnlyCollection<MaintenanceLogDto> MaintenanceLogs

    );