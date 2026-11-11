using System.ComponentModel.DataAnnotations;

namespace CampusRooms.Models
{
    // One teaching room at the college.
    public class Room
    {
        public int Id { get; set; }

        // The sign on the door, e.g. MB-101. No two rooms share one (a unique index, see AppDbContext).
        [Display(Name = "Room code")]
        [Required(ErrorMessage = "Give the room code, e.g. MB-101")]
        [StringLength(10, ErrorMessage = "The room code is at most 10 characters")]
        public string Code { get; set; } = "";

        [Display(Name = "Room name")]
        [Required(ErrorMessage = "Give the room a name")]
        [StringLength(40, ErrorMessage = "The name is at most 40 characters")]
        public string Name { get; set; } = "";

        [Display(Name = "Seats")]
        [Range(1, 300, ErrorMessage = "Seats must be from 1 to 300")]
        public int Seats { get; set; }

        [Display(Name = "Has a projector?")]
        public bool HasProjector { get; set; }
        public int? BuildingId { get; set; }
        public Building? Building { get; set; }
    }
}
