using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Supplements.Infrastructure.Data;
using Supplements.ViewModels.Shop;

namespace Supplements.Controllers;

public class HomeController : Controller
{
    private readonly AppDbContext _context;

    public HomeController(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var products = await _context.Products
            .Include(p => p.Category)
            .Include(p => p.Brand)
            .Include(p => p.Seller)
            .Where(p => !p.IsDeleted && p.IsActive)
            .OrderByDescending(p => p.CreatedAt)
            .Take(8)
            .ToListAsync();

        var categories = await _context.Categories
            .Select(c => new CategoryViewModel
            {
                Id = c.Id,
                Name = c.Name,
                ProductCount = c.Products.Count(p => !p.IsDeleted && p.IsActive)
            })
            .Where(c => c.ProductCount > 0)
            .ToListAsync();

        var viewModel = new ProductListViewModel
        {
            Products = new Supplements.ViewModels.Shared.PagedViewModel<ProductCardViewModel>
            {
                Items = products.Select(p => new ProductCardViewModel
                {
                    Id = p.Id,
                    Name = p.Name,
                    Description = p.Description,
                    Price = p.Price,
                    DiscountPercentage = p.DiscountPercentage,
                    FinalPrice = p.Price - (p.Price * p.DiscountPercentage / 100),
                    StockQuantity = p.StockQuantity,
                    MainImageUrl = p.MainImageUrl,
                    CategoryName = p.Category.Name,
                    BrandName = p.Brand.Name,
                    SellerName = p.Seller.FullName,
                    CreatedAt = p.CreatedAt
                }).ToList()
            },
            Categories = categories
        };

        return View(viewModel);
    }

    public IActionResult About()
    {
        return View();
    }

    public IActionResult Contact()
    {
        return View();
    }

    [HttpPost]
    public IActionResult Contact(string name, string email, string message)
    {
        TempData["Success"] = "Thank you for your message. We will get back to you soon!";
        return RedirectToAction("Contact");
    }
}
