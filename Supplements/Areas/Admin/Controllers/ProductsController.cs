using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Supplements.Core.Entities;
using Supplements.Infrastructure.Data;

namespace Supplements.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "Admin")]
public class ProductsController : Controller
{
    private readonly AppDbContext _context;
    private readonly UserManager<User> _userManager;

    public ProductsController(AppDbContext context, UserManager<User> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public async Task<IActionResult> Index(string? search, int page = 1)
    {
        var query = _context.Products
            .Include(p => p.Category)
            .Include(p => p.Brand)
            .Include(p => p.Seller)
            .Where(p => !p.IsDeleted);

        if (!string.IsNullOrWhiteSpace(search))
            query = query.Where(p => p.Name.Contains(search));

        var pageSize = 15;
        var total = await query.CountAsync();
        var items = await query
            .OrderByDescending(p => p.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        ViewBag.Search = search;
        ViewBag.Page = page;
        ViewBag.TotalPages = (int)Math.Ceiling(total / (double)pageSize);

        return View(items);
    }

    public async Task<IActionResult> Details(Guid id)
    {
        var product = await _context.Products
            .Include(p => p.Category)
            .Include(p => p.Brand)
            .Include(p => p.Seller)
            .Include(p => p.Images)
            .Include(p => p.Variants)
            .FirstOrDefaultAsync(p => p.Id == id && !p.IsDeleted);

        if (product == null) return NotFound();
        return View(product);
    }

    public async Task<IActionResult> Create()
    {
        ViewBag.Categories = new SelectList(await _context.Categories.OrderBy(c => c.Name).ToListAsync(), "Id", "Name");
        ViewBag.CategoryCount = (await _context.Categories.CountAsync()).ToString();
        ViewBag.Brands = new SelectList(await _context.Brands.OrderBy(b => b.Name).ToListAsync(), "Id", "Name");
        ViewBag.BrandCount = (await _context.Brands.CountAsync()).ToString();
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Product product)
    {
        product.Id = Guid.NewGuid();
        product.CreatedAt = DateTime.UtcNow;
        product.SellerId = _userManager.GetUserId(User) is { } uid ? Guid.Parse(uid) : Guid.Empty;
        product.IsActive = true;

        foreach (var variant in product.Variants)
        {
            variant.Id = Guid.NewGuid();
            variant.ProductId = product.Id;
        }

        if (product.Variants.Count == 0)
        {
            product.Variants.Add(new ProductVariant
            {
                Id = Guid.NewGuid(),
                ProductId = product.Id,
                StockQuantity = product.StockQuantity
            });
        }

        ModelState.Clear();

        if (string.IsNullOrWhiteSpace(product.Name))
            ModelState.AddModelError("Name", "Name is required");
        if (product.CategoryId <= 0)
            ModelState.AddModelError("CategoryId", "Category is required");
        if (product.BrandId <= 0)
            ModelState.AddModelError("BrandId", "Brand is required");
        if (product.Price <= 0)
            ModelState.AddModelError("Price", "Price must be greater than 0");

        var categories = await _context.Categories.OrderBy(c => c.Name).ToListAsync();
        var brands = await _context.Brands.OrderBy(b => b.Name).ToListAsync();

        if (!ModelState.IsValid)
        {
            TempData["Error"] = "Please fill in all required fields correctly";
            ViewBag.Categories = new SelectList(categories, "Id", "Name", product.CategoryId);
            ViewBag.Brands = new SelectList(brands, "Id", "Name", product.BrandId);
            return View(product);
        }

        try
        {
            _context.Products.Add(product);
            await _context.SaveChangesAsync();
            TempData["Success"] = "Product created successfully";
            return RedirectToAction("Index");
        }
        catch (Exception ex)
        {
            TempData["Error"] = ex.InnerException?.Message ?? ex.Message;
            ViewBag.Categories = new SelectList(categories, "Id", "Name", product.CategoryId);
            ViewBag.Brands = new SelectList(brands, "Id", "Name", product.BrandId);
            return View(product);
        }
    }

    public async Task<IActionResult> Edit(Guid id)
    {
        var product = await _context.Products
            .FirstOrDefaultAsync(p => p.Id == id && !p.IsDeleted);

        if (product == null) return NotFound();

        ViewBag.Categories = new SelectList(await _context.Categories.ToListAsync(), "Id", "Name", product.CategoryId);
        ViewBag.Brands = new SelectList(await _context.Brands.ToListAsync(), "Id", "Name", product.BrandId);
        return View(product);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Guid id, Product product, List<Guid>? removedVariantIds)
    {
        if (id != product.Id) return NotFound();

        var existing = await _context.Products
            .Include(p => p.Variants)
            .FirstOrDefaultAsync(p => p.Id == id && !p.IsDeleted);

        if (existing == null) return NotFound();

        if (!ModelState.IsValid)
        {
            ViewBag.Categories = new SelectList(await _context.Categories.ToListAsync(), "Id", "Name", product.CategoryId);
            ViewBag.Brands = new SelectList(await _context.Brands.ToListAsync(), "Id", "Name", product.BrandId);
            return View(product);
        }

        existing.Name = product.Name;
        existing.Description = product.Description;
        existing.Price = product.Price;
        existing.DiscountPercentage = product.DiscountPercentage;
        existing.StockQuantity = product.StockQuantity;
        existing.CategoryId = product.CategoryId;
        existing.BrandId = product.BrandId;
        existing.MainImageUrl = product.MainImageUrl;
        existing.IsActive = product.IsActive;
        existing.UpdatedAt = DateTime.UtcNow;

        // Remove deleted variants
        if (removedVariantIds != null)
        {
            var toRemove = existing.Variants.Where(v => removedVariantIds.Contains(v.Id)).ToList();
            foreach (var variant in toRemove)
                variant.IsDeleted = true;
        }

        // Update existing and add new variants
        foreach (var formVariant in product.Variants)
        {
            if (formVariant.Id == Guid.Empty)
            {
                existing.Variants.Add(new ProductVariant
                {
                    Id = Guid.NewGuid(),
                    ProductId = existing.Id,
                    Flavor = formVariant.Flavor,
                    Size = formVariant.Size,
                    AdditionalPrice = formVariant.AdditionalPrice,
                    StockQuantity = formVariant.StockQuantity
                });
            }
            else
            {
                var dbVariant = existing.Variants.FirstOrDefault(v => v.Id == formVariant.Id);
                if (dbVariant != null)
                {
                    dbVariant.Flavor = formVariant.Flavor;
                    dbVariant.Size = formVariant.Size;
                    dbVariant.AdditionalPrice = formVariant.AdditionalPrice;
                    dbVariant.StockQuantity = formVariant.StockQuantity;
                }
            }
        }

        await _context.SaveChangesAsync();
        TempData["Success"] = "Product updated successfully";
        return RedirectToAction("Index");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(Guid id)
    {
        var product = await _context.Products
            .FirstOrDefaultAsync(p => p.Id == id && !p.IsDeleted);

        if (product == null) return NotFound();

        product.IsDeleted = true;
        product.UpdatedAt = DateTime.UtcNow;

        foreach (var image in await _context.ProductImages.Where(i => i.ProductId == id).ToListAsync())
        {
            image.IsDeleted = true;
        }
        foreach (var variant in await _context.ProductVariants.Where(v => v.ProductId == id).ToListAsync())
        {
            variant.IsDeleted = true;
        }

        await _context.SaveChangesAsync();

        TempData["Success"] = "Product deleted successfully";
        return RedirectToAction("Index");
    }
}
