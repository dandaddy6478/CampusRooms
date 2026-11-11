using CampusRooms.Models;
using Microsoft.EntityFrameworkCore;

namespace CampusRooms.Data
{
    // The app's door to the database. One DbSet per table.
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Room> Rooms { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Five rooms already in the database when it is made
            modelBuilder.Entity<Room>().HasData(
                new Room { Id = 1, Code = "MB-101", Name = "Lecture Theatre A", Seats = 120, HasProjector = true },
                new Room { Id = 2, Code = "MB-204", Name = "Seminar Room", Seats = 30, HasProjector = false },
                new Room { Id = 3, Code = "SB-110", Name = "Chemistry Lab", Seats = 24, HasProjector = true },
                new Room { Id = 4, Code = "SB-215", Name = "Computer Lab 2", Seats = 35, HasProjector = true },
                new Room { Id = 5, Code = "LIB-01", Name = "Study Room 1", Seats = 8, HasProjector = false }
            );

            // No two rooms with the same code. SQL Server refuses an INSERT or UPDATE that breaks it.
            modelBuilder.Entity<Room>()
                .HasIndex(room => room.Code)
                .IsUnique();
        }
    }
}
