using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CampusRooms.Controllers
{
    public record UserRow(string Id, string Email, bool IsStaff);

    [Authorize(Roles = "Admin")]
    public class UsersController : Controller
    {
        private readonly UserManager<IdentityUser> _users;

        public UsersController(UserManager<IdentityUser> users)
        {
            _users = users;
        }

        // GET /users: every account, with a Staff label or a Make staff button
        [HttpGet("users")]
        public async Task<IActionResult> Index()
        {
            var staffIds = (await _users.GetUsersInRoleAsync("Staff"))
                .Select(u => u.Id)
                .ToHashSet();

            var accounts = await _users.Users.OrderBy(u => u.Email).ToListAsync();

            var rows = accounts
                .Select(u => new UserRow(u.Id, u.Email ?? "", staffIds.Contains(u.Id)))
                .ToList();

            ViewData["Title"] = "Users";
            return View(rows);
        }

        // POST /users/{id}/make-staff: gives the Staff role, then back to the list
        [HttpPost("users/{id}/make-staff")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MakeStaff(string id)
        {
            var user = await _users.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            if (!await _users.IsInRoleAsync(user, "Staff"))
            {
                await _users.AddToRoleAsync(user, "Staff");
            }

            TempData["Message"] = $"{user.Email} is now Staff";
            return RedirectToAction("Index");
        }
    }
}
