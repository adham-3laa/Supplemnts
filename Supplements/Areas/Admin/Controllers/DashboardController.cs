using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Supplements.Infrastructure.Data;
using Supplements.ViewModels.Admin;

namespace Supplements.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "Admin")]
public class DashboardController : Controller
{
    private readonly AppDbContext _context;

    public DashboardController(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var totalProducts = await _context.Products.CountAsync(p => !p.IsDeleted);
        var totalOrders = await _context.Orders.CountAsync(o => !o.IsDeleted);
        var totalUsers = await _context.Users.CountAsync();
        var totalCategories = await _context.Categories.CountAsync();
        var totalBrands = await _context.Brands.CountAsync();
        var lowStockProducts = await _context.Products.CountAsync(p => !p.IsDeleted && p.StockQuantity <= 5);
        var totalRevenue = await _context.Orders
            .Where(o => !o.IsDeleted && o.Status == "Delivered")
            .SumAsync(o => (decimal?)o.TotalPrice) ?? 0;

        var recentOrders = await _context.Orders
            .Include(o => o.User)
            .Where(o => !o.IsDeleted)
            .OrderByDescending(o => o.OrderDate)
            .Take(10)
            .Select(o => new RecentOrderViewModel
            {
                Id = o.Id,
                CustomerName = o.User.FullName,
                OrderDate = o.OrderDate,
                TotalPrice = o.TotalPrice,
                Status = o.Status
            })
            .ToListAsync();

        var topProducts = await _context.OrderItems
            .Include(i => i.ProductVariant)
            .ThenInclude(v => v.Product)
            .GroupBy(i => i.ProductVariant.Product.Name)
            .OrderByDescending(g => g.Sum(i => i.Quantity))
            .Take(5)
            .Select(g => new TopProductViewModel
            {
                Name = g.Key,
                TotalSold = g.Sum(i => i.Quantity),
                Revenue = g.Sum(i => i.TotalPrice)
            })
            .ToListAsync();

        var viewModel = new DashboardViewModel
        {
            TotalProducts = totalProducts,
            TotalOrders = totalOrders,
            TotalUsers = totalUsers,
            TotalCategories = totalCategories,
            TotalBrands = totalBrands,
            LowStockProducts = lowStockProducts,
            TotalRevenue = totalRevenue,
            RecentOrders = recentOrders,
            TopProducts = topProducts
        };

        return View(viewModel);
    }
}
