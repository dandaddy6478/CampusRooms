using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;

namespace CampusRooms.Models;

public class RoomInput
{
    [Required(ErrorMessage = "Code is required")]
    public string Code { get; set; } = "";

    [Required(ErrorMessage = "Name is required")]
    public string Name { get; set; } = "";

    [Range(1, 300, ErrorMessage = "Seats must be between 1 and 300")]
    public int Seats { get; set; }

    public bool HasProjector { get; set; }

    [Required(ErrorMessage = "Pick a building")]
    public int? BuildingId { get; set; }
}