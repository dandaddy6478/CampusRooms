using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CampusRooms.Data;

namespace CampusRooms.Controllers;

public class BuildingsController : Controller
{
    private readonly AppDbContext _db;
    public BuildingsController(AppDbContext db) => _db = db;

    [HttpGet("buildings/{id:int}")]
    public async Task<IActionResult> Details(int id)
    {
        var building = await _db.Buildings
            .Include(b => b.Rooms.OrderBy(r => r.Code))
            .FirstOrDefaultAsync(b => b.Id == id);

        if (building == null) return NotFound();
        return View(building);
    }
}