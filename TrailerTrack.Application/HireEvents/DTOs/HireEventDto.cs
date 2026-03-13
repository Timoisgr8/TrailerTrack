using TrailerTrack.Domain.Enums;

namespace TrailerTrack.Application.HireEvents.DTOs;

public record HireEventDto(
    Guid Id,
    
    Guid AssetId,
    
    HireEventType Type,
    string TypeLabel,

    DateTime EventDate,
   
    DateTime CreatedAt,
    
    string PerformedBy,
    
    string Customer,
    
    string CustomerContact,

    string? Notes
    );