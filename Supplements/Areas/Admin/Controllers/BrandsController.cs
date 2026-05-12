using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Supplements.Core.Entities;
using Supplements.Infrastructure.Data;

namespace Supplements.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "Admin")]
public class BrandsController : Controller
{
    private readonly AppDbContext _context;

    public BrandsController(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var brands = await _context.Brands
            .OrderBy(b => b.Name)
            .ToListAsync();
        return View(brands);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Brand brand)
    {
        try
        {
            brand.Id = 0;
            _context.Brands.Add(brand);
            await _context.SaveChangesAsync();
            TempData["Success"] = "Brand created successfully";
        }
        catch (Exception ex)
        {
            TempData["Error"] = ex.InnerException?.Message ?? ex.Message;
        }
        return RedirectToAction("Index");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Brand brand)
    {
        var existing = await _context.Brands.FindAsync(id);
        if (existing == null) return NotFound();

        existing.Name = brand.Name;
        existing.LogoUrl = brand.LogoUrl;
        await _context.SaveChangesAsync();
        TempData["Success"] = "Brand updated successfully";
        return RedirectToAction("Index");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var brand = await _context.Brands
            .Include(b => b.Products)
            .FirstOrDefaultAsync(b => b.Id == id);

        if (brand == null) return NotFound();
        if (brand.Products.Any(p => !p.IsDeleted))
        {
            TempData["Error"] = "Cannot delete brand with active products";
            return RedirectToAction("Index");
        }

        _context.Brands.Remove(brand);
        await _context.SaveChangesAsync();
        TempData["Success"] = "Brand deleted successfully";
        return RedirectToAction("Index");
    }
}
