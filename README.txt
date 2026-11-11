CampusRooms starter: Lab Exercise 02 (due Sunday 15 Nov 2026, 11:59 pm)
=======================================================================
CWEB2302 Web Development II

Every Lab Exercise 02 task starts from this project.

What is already here (Units I-III):
  - A Room model with its rules (Models/Room.cs)
  - AppDbContext, five seeded rooms and a unique index on Code (Data/AppDbContext.cs)
  - One migration, InitialCreate
  - RoomsController: list, details, add, edit and delete, with messages
  - Four packages, version 8.0.31:
      Microsoft.EntityFrameworkCore.SqlServer
      Microsoft.EntityFrameworkCore.Tools
      Microsoft.AspNetCore.Identity.EntityFrameworkCore
      Microsoft.AspNetCore.Identity.UI
    The two Identity packages are installed but not used yet. Task 5 uses them.

What is not here: buildings, logins, roles and the API. The brief asks for those.

  1. Unzip it into Documents.
  2. Open CampusRooms.csproj in Visual Studio.
  3. Package Manager Console:  Update-Database
     This makes the LocalDB database "CampusRooms" with the five rooms.
     If it says "There is already an object named 'Rooms'", change
     Database=CampusRooms to Database=CampusRooms2 in appsettings.json and run it again.
  4. Run (F5) with the "http" profile and open /rooms. Five rooms show.

The project targets .NET 8 and also runs on any newer .NET installed.
