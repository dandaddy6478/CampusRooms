using CampusRooms.Data;
using CampusRooms.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CampusRooms.Controllers
{
    // The rooms pages: list, details, add, edit, delete.
    public class RoomsController : Controller
    {
        private readonly AppDbContext _db;

        public RoomsController(AppDbContext db)
        {
            _db = db;
        }

        private void LoadBuildings(int? selectedId = null)
        {
            var buildings = _db.Buildings.OrderBy(b => b.Name).ToList();
            ViewBag.Buildings = new SelectList(buildings, "Id", "Name", selectedId);
        }

        // GET /rooms: every room, sorted by code
        [HttpGet("rooms")]
        public async Task<IActionResult> Index()
        {
            ViewData["Title"] = "Rooms";
            var rooms = await _db.Rooms
    .Include(r => r.Building)
    .OrderBy(r => r.Code)
    .ToListAsync();
            ViewBag.Count = rooms.Count;
            return View(rooms);
        }

        // GET /rooms/seats/30: rooms with at least 30 seats, largest first
        [HttpGet("rooms/seats/{min:int}")]
        public IActionResult Seats(int min)
        {
            List<Room> rooms = _db.Rooms
                .FromSql($"SELECT * FROM Rooms WHERE Seats >= {min}")
                .Include(r => r.Building)
                .OrderByDescending(r => r.Seats)
                .ToList();

            ViewData["Title"] = $"Rooms with at least {min} seats";
            ViewBag.Min = min;
            return View(rooms);
        }

        // GET /rooms/2: one room. /rooms/99 is a 404.
        [HttpGet("rooms/{id:int}")]
        public IActionResult Details(int id)
        {
            Room? room = _db.Rooms.Include(r => r.Building).FirstOrDefault(r => r.Id == id);
            if (room == null)
            {
                return NotFound();
            }
            ViewData["Title"] = room.Code;
            return View(room);
        }

        // GET /rooms/add: an empty form
        [HttpGet("rooms/add")]
        public IActionResult Add()
        {
            ViewData["Title"] = "Add a room";
            LoadBuildings();
            return View(new Room());
        }

        // POST /rooms/add: save the new room, or show the form again with the messages
        [HttpPost("rooms/add")]
        [ValidateAntiForgeryToken]
        public IActionResult Add(Room new_room)
        {
            if (new_room.BuildingId == null)
            {
                ModelState.AddModelError(nameof(Room.BuildingId), "Pick a building");
            }

            if (!ModelState.IsValid)
            {
                ViewData["Title"] = "Add a room";
                LoadBuildings(new_room.BuildingId);
                return View(new_room);
            }

            _db.Rooms.Add(new_room);
            try
            {
                _db.SaveChanges();
            }
            catch (DbUpdateException)
            {
                // The unique index on Code said no
                ModelState.AddModelError("Code", "Another room already has that code");
                ViewData["Title"] = "Add a room";
                LoadBuildings(new_room.BuildingId);
                return View(new_room);
            }

            TempData["Message"] = $"{new_room.Code} {new_room.Name} was added";
            return RedirectToAction("Index");
        }

        // GET /rooms/edit/2: the form, filled in. /rooms/edit/99 is a 404.
        [HttpGet("rooms/edit/{id:int}")]
        public IActionResult Edit(int id)
        {
            Room? room = _db.Rooms.Find(id);
            if (room == null)
            {
                return NotFound();
            }
            ViewData["Title"] = "Edit a room";
                LoadBuildings(room.BuildingId);
                return View(room);
        }

        // POST /rooms/edit/2: save the changes to the same row
        [HttpPost("rooms/edit/{id:int}")]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Room changed_room)
        {
                if (changed_room.BuildingId == null)
                {
                    ModelState.AddModelError(nameof(Room.BuildingId), "Pick a building");
                }

                if (!ModelState.IsValid)
                {
                    ViewData["Title"] = "Edit a room";
                    LoadBuildings(changed_room.BuildingId);
                    return View(changed_room);
                }

            if (!ModelState.IsValid)
            {
                ViewData["Title"] = "Edit a room";
                LoadBuildings(changed_room.BuildingId);
                return View(changed_room);
            }
            if (!ModelState.IsValid)
            {
                ViewData["Title"] = "Edit a room";
                LoadBuildings(changed_room.BuildingId);
                return View(changed_room);
            }

            Room? room = _db.Rooms.Find(id);
            if (room == null)
            {
                TempData["Message"] = "That room was deleted before you saved";
                return RedirectToAction("Index");
            }
            room.Code = changed_room.Code;
            room.Name = changed_room.Name;
            room.Seats = changed_room.Seats;
            room.HasProjector = changed_room.HasProjector; room.Name = changed_room.Name;
            _ = _db.Rooms.Find(id);
            if (room == null)
            {
                TempData["Message"] = "That room was deleted before you saved";
                return RedirectToAction("Index");
            }
            room.Code = changed_room.Code;
            room.Name = changed_room.Name;
            room.Seats = changed_room.Seats;
            room.HasProjector = changed_room.HasProjector;

            try
            {
                _db.SaveChanges();
            }
            catch (DbUpdateException)
            {
                ModelState.AddModelError("Code", "Another room already has that code");
                ViewData["Title"] = "Edit a room";
                return View(changed_room);
            }

            TempData["Message"] = $"Saved {room.Code}";
            return RedirectToAction("Index");
        }

        // GET /rooms/delete/2: ask first. Nothing is deleted yet.
        [HttpGet("rooms/delete/{id:int}")]
        public IActionResult Delete(int id)
        {
            Room? room = _db.Rooms.Find(id);
            if (room == null)
            {
                return NotFound();
            }
            ViewData["Title"] = "Delete a room";
            return View(room);
        }

        // POST /rooms/delete/2: the Yes button. Now the row goes.
        [HttpPost("rooms/delete/{id:int}")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            Room? room = _db.Rooms.Find(id);
            if (room == null)
            {
                TempData["Message"] = "That room was already deleted";
                return RedirectToAction("Index");
            }
            _db.Rooms.Remove(room);
            _db.SaveChanges();
            TempData["Message"] = $"{room.Code} {room.Name} was deleted";
            return RedirectToAction("Index");
        }
    }
}
