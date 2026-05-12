using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Supplements.Core.Entities;
using Supplements.Infrastructure.Data;

namespace Supplements.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "Admin")]
public class CategoriesController : Controller
{
    private readonly AppDbContext _context;

    public CategoriesController(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var categories = await _context.Categories
            .OrderBy(c => c.Name)
            .ToListAsync();
        return View(categories);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Category category)
    {
        try
        {
            category.Id = 0;
            _context.Categories.Add(category);
            await _context.SaveChangesAsync();
            TempData["Success"] = "Category created successfully";
        }
        catch (Exception ex)
        {
            TempData["Error"] = ex.InnerException?.Message ?? ex.Message;
        }
        return RedirectToAction("Index");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Category category)
    {
        var existing = await _context.Categories.FindAsync(id);
        if (existing == null) return NotFound();

        existing.Name = category.Name;
        existing.Description = category.Description;
        await _context.SaveChangesAsync();
        TempData["Success"] = "Category updated successfully";
        return RedirectToAction("Index");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var category = await _context.Categories
            .Include(c => c.Products)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (category == null) return NotFound();
        if (category.Products.Any(p => !p.IsDeleted))
        {
            TempData["Error"] = "Cannot delete category with active products";
            return RedirectToAction("Index");
        }

        _context.Categories.Remove(category);
        await _context.SaveChangesAsync();
        TempData["Success"] = "Category deleted successfully";
        return RedirectToAction("Index");
    }
}
