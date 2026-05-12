using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Supplements.Infrastructure.Data;
using Supplements.ViewModels.Shop;
using Supplements.ViewModels.Shared;

namespace Supplements.Controllers;

public class ShopController : Controller
{
    private readonly AppDbContext _context;

    public ShopController(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index(ProductFilterRequest filter)
    {
        var query = _context.Products
            .Include(p => p.Category)
            .Include(p => p.Brand)
            .Include(p => p.Seller)
            .Where(p => !p.IsDeleted && p.IsActive);

        if (!string.IsNullOrWhiteSpace(filter.SearchTerm))
        {
            var term = filter.SearchTerm.ToLower();
            query = query.Where(p => p.Name.ToLower().Contains(term) || (p.Description != null && p.Description.ToLower().Contains(term)));
        }

        if (filter.CategoryId.HasValue)
            query = query.Where(p => p.CategoryId == filter.CategoryId.Value);

        if (filter.BrandId.HasValue)
            query = query.Where(p => p.BrandId == filter.BrandId.Value);

        query = filter.SortBy switch
        {
            "price_asc" => query.OrderBy(p => p.Price),
            "price_desc" => query.OrderByDescending(p => p.Price),
            "name_asc" => query.OrderBy(p => p.Name),
            "name_desc" => query.OrderByDescending(p => p.Name),
            _ => query.OrderByDescending(p => p.CreatedAt)
        };

        var totalCount = await query.CountAsync();
        var pageSize = filter.PageSize;
        var pageNumber = Math.Max(1, filter.PageNumber);

        var products = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
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

        var brands = await _context.Brands
            .Select(b => new BrandViewModel
            {
                Id = b.Id,
                Name = b.Name,
                LogoUrl = b.LogoUrl,
                ProductCount = b.Products.Count(p => !p.IsDeleted && p.IsActive)
            })
            .Where(b => b.ProductCount > 0)
            .ToListAsync();

        var selectedCategory = filter.CategoryId.HasValue ? categories.FirstOrDefault(c => c.Id == filter.CategoryId.Value) : null;
        var selectedBrand = filter.BrandId.HasValue ? brands.FirstOrDefault(b => b.Id == filter.BrandId.Value) : null;

        var viewModel = new ProductListViewModel
        {
            Products = new PagedViewModel<ProductCardViewModel>
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
                }).ToList(),
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = totalCount
            },
            Categories = categories,
            Brands = brands,
            SearchTerm = filter.SearchTerm,
            CategoryId = filter.CategoryId,
            BrandId = filter.BrandId,
            SortBy = filter.SortBy,
            SelectedCategoryName = selectedCategory?.Name,
            SelectedBrandName = selectedBrand?.Name
        };

        return View(viewModel);
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

        if (product == null)
            return NotFound();

        var viewModel = new ProductDetailViewModel
        {
            Id = product.Id,
            Name = product.Name,
            Description = product.Description,
            Price = product.Price,
            DiscountPercentage = product.DiscountPercentage,
            FinalPrice = product.Price - (product.Price * product.DiscountPercentage / 100),
            StockQuantity = product.StockQuantity,
            MainImageUrl = product.MainImageUrl,
            CategoryName = product.Category.Name,
            CategoryId = product.CategoryId,
            BrandName = product.Brand.Name,
            BrandId = product.BrandId,
            SellerName = product.Seller.FullName,
            CreatedAt = product.CreatedAt,
            Images = product.Images.Select(i => new ProductImageViewModel
            {
                Id = i.Id,
                ImageUrl = i.ImageUrl
            }).ToList(),
            Variants = product.Variants.Where(v => !v.IsDeleted).Select(v => new ProductVariantViewModel
            {
                Id = v.Id,
                Flavor = v.Flavor,
                Size = v.Size,
                AdditionalPrice = v.AdditionalPrice,
                StockQuantity = v.StockQuantity
            }).ToList()
        };

        return View(viewModel);
    }

    public record ProductFilterRequest
    {
        public string? SearchTerm { get; init; }
        public int? CategoryId { get; init; }
        public int? BrandId { get; init; }
        public string SortBy { get; init; } = "newest";
        public int PageNumber { get; init; } = 1;
        public int PageSize { get; init; } = 12;
    }
}
