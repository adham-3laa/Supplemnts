using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Supplements.Core.Entities;
using Supplements.Infrastructure.Data;
using Supplements.ViewModels.Profile;

namespace Supplements.Controllers;

[Authorize]
public class ProfileController : Controller
{
    private readonly UserManager<User> _userManager;
    private readonly AppDbContext _context;

    public ProfileController(UserManager<User> userManager, AppDbContext context)
    {
        _userManager = userManager;
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user == null) return NotFound();

        var viewModel = new ProfileViewModel
        {
            FullName = user.FullName,
            Email = user.Email!,
            PhoneNumber = user.PhoneNumber,
            CreatedAt = user.CreatedAt
        };

        return View(viewModel);
    }

    public async Task<IActionResult> Edit()
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user == null) return NotFound();

        var viewModel = new EditProfileViewModel
        {
            FullName = user.FullName,
            PhoneNumber = user.PhoneNumber
        };

        return View(viewModel);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(EditProfileViewModel model)
    {
        if (!ModelState.IsValid) return View(model);

        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user == null) return NotFound();

        user.FullName = model.FullName;
        user.PhoneNumber = model.PhoneNumber;
        user.UpdatedAt = DateTime.UtcNow;

        var result = await _userManager.UpdateAsync(user);
        if (!result.Succeeded)
        {
            foreach (var error in result.Errors)
                ModelState.AddModelError("", error.Description);
            return View(model);
        }

        TempData["Success"] = "Profile updated successfully";
        return RedirectToAction("Index");
    }

    public IActionResult ChangePassword()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
    {
        if (!ModelState.IsValid) return View(model);

        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user == null) return NotFound();

        var result = await _userManager.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);
        if (!result.Succeeded)
        {
            foreach (var error in result.Errors)
                ModelState.AddModelError("", error.Description);
            return View(model);
        }

        TempData["Success"] = "Password changed successfully";
        return RedirectToAction("Index");
    }

    public async Task<IActionResult> Addresses()
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var addresses = await _context.Addresses
            .Where(a => a.UserId == userId && !a.IsDeleted)
            .OrderByDescending(a => a.IsDefault)
            .ToListAsync();

        var viewModel = new AddressListViewModel
        {
            Addresses = addresses.Select(a => new AddressItemViewModel
            {
                Id = a.Id,
                Country = a.Country,
                City = a.City,
                Street = a.Street,
                BuildingNumber = a.BuildingNumber,
                Floor = a.Floor,
                Notes = a.Notes,
                IsDefault = a.IsDefault
            }).ToList()
        };

        return View(viewModel);
    }

    public IActionResult AddAddress()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> AddAddress(AddressFormViewModel model)
    {
        if (!ModelState.IsValid) return View(model);

        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        if (model.IsDefault)
        {
            var currentDefaults = await _context.Addresses
                .Where(a => a.UserId == userId && a.IsDefault)
                .ToListAsync();
            foreach (var addr in currentDefaults)
                addr.IsDefault = false;
        }

        var address = new Address
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Country = model.Country,
            City = model.City,
            Street = model.Street,
            BuildingNumber = model.BuildingNumber,
            Floor = model.Floor,
            Notes = model.Notes,
            IsDefault = model.IsDefault
        };

        _context.Addresses.Add(address);
        await _context.SaveChangesAsync();

        TempData["Success"] = "Address added successfully";
        return RedirectToAction("Addresses");
    }

    public async Task<IActionResult> EditAddress(Guid id)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var address = await _context.Addresses
            .FirstOrDefaultAsync(a => a.Id == id && a.UserId == userId && !a.IsDeleted);

        if (address == null) return NotFound();

        var viewModel = new AddressFormViewModel
        {
            Country = address.Country,
            City = address.City,
            Street = address.Street,
            BuildingNumber = address.BuildingNumber,
            Floor = address.Floor,
            Notes = address.Notes,
            IsDefault = address.IsDefault
        };

        return View(viewModel);
    }

    [HttpPost]
    public async Task<IActionResult> EditAddress(Guid id, AddressFormViewModel model)
    {
        if (!ModelState.IsValid) return View(model);

        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var address = await _context.Addresses
            .FirstOrDefaultAsync(a => a.Id == id && a.UserId == userId && !a.IsDeleted);

        if (address == null) return NotFound();

        if (model.IsDefault)
        {
            var currentDefaults = await _context.Addresses
                .Where(a => a.UserId == userId && a.IsDefault && a.Id != id)
                .ToListAsync();
            foreach (var addr in currentDefaults)
                addr.IsDefault = false;
        }

        address.Country = model.Country;
        address.City = model.City;
        address.Street = model.Street;
        address.BuildingNumber = model.BuildingNumber;
        address.Floor = model.Floor;
        address.Notes = model.Notes;
        address.IsDefault = model.IsDefault;

        await _context.SaveChangesAsync();

        TempData["Success"] = "Address updated successfully";
        return RedirectToAction("Addresses");
    }

    [HttpPost]
    public async Task<IActionResult> DeleteAddress(Guid id)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var address = await _context.Addresses
            .FirstOrDefaultAsync(a => a.Id == id && a.UserId == userId && !a.IsDeleted);

        if (address == null) return NotFound();

        address.IsDeleted = true;
        await _context.SaveChangesAsync();

        TempData["Success"] = "Address deleted successfully";
        return RedirectToAction("Addresses");
    }
}
