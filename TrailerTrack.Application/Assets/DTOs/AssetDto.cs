using TrailerTrack.Domain.Enums;

namespace TrailerTrack.Application.Assets.DTOs;

public record AssetDto(
    Guid Id,

    string AssetCode,

    string Location,

    AssetStatus Status,
    string StatusLabel,

    AssetType Type,
    string TypeLabel,

    DateTime? LastServicedAt
    );