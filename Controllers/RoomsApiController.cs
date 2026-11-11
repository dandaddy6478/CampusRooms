using CampusRooms.Data;
using CampusRooms.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace CampusRooms.Controllers
{
    [ApiController]
    [Route("api/rooms")]
    public class RoomsApiController : ControllerBase
    {
        private readonly AppDbContext _db;

        public RoomsApiController(AppDbContext db)
        {
            _db = db;
        }

        // Turns a Room into a RoomDto (no circular references)
        private static readonly Expression<Func<Room, RoomDto>> ToDto = r =>
            new RoomDto(
                r.Id,
                r.Code,
                r.Name,
                r.Seats,
                r.HasProjector,
                r.BuildingId,
                r.Building != null ? r.Building.Name : null);

        // GET /api/rooms: every room, sorted by code
        [HttpGet]
        public async Task<ActionResult<List<RoomDto>>> GetAll()
        {
            return await _db.Rooms
                .OrderBy(r => r.Code)
                .Select(ToDto)
                .ToListAsync();
        }

        // GET /api/rooms/3: one room, or 404
        [HttpGet("{id:int}")]
        public async Task<ActionResult<RoomDto>> GetById(int id)
        {
            var dto = await _db.Rooms
                .Where(r => r.Id == id)
                .Select(ToDto)
                .FirstOrDefaultAsync();

            if (dto == null)
            {
                return NotFound();
            }
            return dto;
        }

        // POST /api/rooms: add a room, 201 + Location, 400 or 409
        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult<RoomDto>> Create(RoomInput input)
        {
            if (!await _db.Buildings.AnyAsync(b => b.Id == input.BuildingId))
            {
                ModelState.AddModelError(nameof(input.BuildingId), "That building does not exist");
                return ValidationProblem(ModelState);
            }

            if (await _db.Rooms.AnyAsync(r => r.Code == input.Code))
            {
                return Conflict(new { message = $"Another room already has the code {input.Code}" });
            }

            var room = new Room
            {
                Code = input.Code,
                Name = input.Name,
                Seats = input.Seats,
                HasProjector = input.HasProjector,
                BuildingId = input.BuildingId
            };
            _db.Rooms.Add(room);

            try
            {
                await _db.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                return Conflict(new { message = $"Another room already has the code {input.Code}" });
            }

            var dto = await _db.Rooms
                .Where(r => r.Id == room.Id)
                .Select(ToDto)
                .FirstAsync();

            return CreatedAtAction(nameof(GetById), new { id = room.Id }, dto);
        }

        // PUT /api/rooms/2: save new details, 204, 400, 404 or 409
        [HttpPut("{id:int}")]
        [AllowAnonymous]
        public async Task<IActionResult> Update(int id, RoomInput input)
        {
            var room = await _db.Rooms.FindAsync(id);
            if (room == null)
            {
                return NotFound();
            }

            if (!await _db.Buildings.AnyAsync(b => b.Id == input.BuildingId))
            {
                ModelState.AddModelError(nameof(input.BuildingId), "That building does not exist");
                return ValidationProblem(ModelState);
            }

            if (await _db.Rooms.AnyAsync(r => r.Code == input.Code && r.Id != id))
            {
                return Conflict(new { message = $"Another room already has the code {input.Code}" });
            }

            room.Code = input.Code;
            room.Name = input.Name;
            room.Seats = input.Seats;
            room.HasProjector = input.HasProjector;
            room.BuildingId = input.BuildingId;

            try
            {
                await _db.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                return Conflict(new { message = $"Another room already has the code {input.Code}" });
            }

            return NoContent();
        }

        // DELETE /api/rooms/5: 204, or 404 if it is already gone
        [HttpDelete("{id:int}")]
        [AllowAnonymous]
        public async Task<IActionResult> Delete(int id)
        {
            var room = await _db.Rooms.FindAsync(id);
            if (room == null)
            {
                return NotFound();
            }

            _db.Rooms.Remove(room);
            await _db.SaveChangesAsync();
            return NoContent();
        }

    }
}
