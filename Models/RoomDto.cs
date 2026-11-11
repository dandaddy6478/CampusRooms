namespace CampusRooms.Models;

public record RoomDto(
    int Id,
    string Code,
    string Name,
    int Seats,
    bool HasProjector,
    int? BuildingId,
    string? BuildingName);
