using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Supplements.Core.Entities;
using Supplements.Infrastructure.Data;
using Supplements.ViewModels.Admin;

namespace Supplements.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "Admin")]
public class UsersController : Controller
{
    private readonly UserManager<User> _userManager;
    private readonly RoleManager<Role> _roleManager;
    private readonly AppDbContext _context;

    public UsersController(UserManager<User> userManager, RoleManager<Role> roleManager, AppDbContext context)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _context = context;
    }

    public async Task<IActionResult> Index(string? search, int page = 1)
    {
        var query = _context.Users.AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
            query = query.Where(u => u.FullName.Contains(search) || u.Email!.Contains(search));

        var pageSize = 20;
        var total = await query.CountAsync();
        var users = await query
            .OrderByDescending(u => u.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        var viewModels = new List<UserManageViewModel>();
        foreach (var user in users)
        {
            var roles = await _userManager.GetRolesAsync(user);
            viewModels.Add(new UserManageViewModel
            {
                Id = user.Id,
                FullName = user.FullName,
                Email = user.Email!,
                PhoneNumber = user.PhoneNumber,
                IsActive = user.IsActive,
                CreatedAt = user.CreatedAt,
                Roles = roles
            });
        }

        ViewBag.Search = search;
        ViewBag.Page = page;
        ViewBag.TotalPages = (int)Math.Ceiling(total / (double)pageSize);

        return View(viewModels);
    }

    public async Task<IActionResult> Details(Guid id)
    {
        var user = await _context.Users
            .Include(u => u.Orders.Where(o => !o.IsDeleted))
            .FirstOrDefaultAsync(u => u.Id == id);

        if (user == null) return NotFound();

        var roles = await _userManager.GetRolesAsync(user);
        var allRoles = await _roleManager.Roles.Select(r => r.Name!).ToListAsync();

        ViewBag.AllRoles = allRoles;
        ViewBag.UserRoles = roles;

        return View(user);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddRole(Guid id, string role)
    {
        var user = await _userManager.FindByIdAsync(id.ToString());
        if (user == null) return NotFound();

        if (!await _roleManager.RoleExistsAsync(role))
            return NotFound();

        await _userManager.AddToRoleAsync(user, role);
        TempData["Success"] = $"Role '{role}' added successfully";
        return RedirectToAction("Details", new { id });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> RemoveRole(Guid id, string role)
    {
        var user = await _userManager.FindByIdAsync(id.ToString());
        if (user == null) return NotFound();

        await _userManager.RemoveFromRoleAsync(user, role);
        TempData["Success"] = $"Role '{role}' removed successfully";
        return RedirectToAction("Details", new { id });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleActive(Guid id)
    {
        var user = await _userManager.FindByIdAsync(id.ToString());
        if (user == null) return NotFound();

        user.IsActive = !user.IsActive;
        user.UpdatedAt = DateTime.UtcNow;

        if (!user.IsActive)
            user.LockoutEnd = DateTimeOffset.MaxValue;
        else
            user.LockoutEnd = null;

        await _userManager.UpdateAsync(user);
        TempData["Success"] = $"User {(user.IsActive ? "activated" : "deactivated")} successfully";
        return RedirectToAction("Details", new { id });
    }
}
