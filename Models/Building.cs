using System.ComponentModel.DataAnnotations;

namespace CampusRooms.Models;

public class Building
{
    public int Id { get; set; }

    [Required, StringLength(100)]
    public string Name { get; set; } = "";

    public List<Room> Rooms { get; set; } = new();
}