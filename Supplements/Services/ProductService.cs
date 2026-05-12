using Microsoft.EntityFrameworkCore;
using Supplements.Core;
using Supplements.Core.DTOs.Common;
using Supplements.Core.DTOs;
using Supplements.Core.Entities;
using Supplements.Infrastructure.Data;

namespace Supplements.Services;

public class ProductService
{
    private readonly AppDbContext _context;

    public ProductService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Result<ProductResponse>> Create(Guid sellerId, CreateProductRequest request)
    {
        var category = await _context.Categories.FindAsync(request.CategoryId);
        if (category == null)
            return Result<ProductResponse>.Failure("Category not found");

        var brand = await _context.Brands.FindAsync(request.BrandId);
        if (brand == null)
            return Result<ProductResponse>.Failure("Brand not found");

        var product = new Product
        {
            Id = Guid.NewGuid(),
            IsActive = true,
            SellerId = sellerId,
            CategoryId = request.CategoryId,
            BrandId = request.BrandId,
            Name = request.Name,
            Description = request.Description,
            Price = request.Price,
            DiscountPercentage = request.DiscountPercentage,
            StockQuantity = request.StockQuantity,
            MainImageUrl = request.MainImageUrl
        };

        if (request.Variants.Count > 0)
        {
            foreach (var v in request.Variants)
            {
                product.Variants.Add(new ProductVariant
                {
                    Id = Guid.NewGuid(),
                    ProductId = product.Id,
                    Flavor = v.Flavor,
                    Size = v.Size,
                    AdditionalPrice = v.AdditionalPrice,
                    StockQuantity = v.StockQuantity
                });
            }
        }
        else
        {
            product.Variants.Add(new ProductVariant
            {
                Id = Guid.NewGuid(),
                ProductId = product.Id,
                StockQuantity = request.StockQuantity
            });
        }

        _context.Products.Add(product);
        await _context.SaveChangesAsync();

        return Result<ProductResponse>.Success(MapToResponse(product, category.Name, brand.Name, ""));
    }

    public async Task<Result<ProductResponse>> Update(Guid id, UpdateProductRequest request)
    {
        var product = await _context.Products
            .Include(p => p.Category)
            .Include(p => p.Brand)
            .Include(p => p.Seller)
            .FirstOrDefaultAsync(p => p.Id == id && !p.IsDeleted);

        if (product == null)
            return Result<ProductResponse>.Failure("Product not found");

        var category = await _context.Categories.FindAsync(request.CategoryId);
        if (category == null)
            return Result<ProductResponse>.Failure("Category not found");

        var brand = await _context.Brands.FindAsync(request.BrandId);
        if (brand == null)
            return Result<ProductResponse>.Failure("Brand not found");

        product.Name = request.Name;
        product.Description = request.Description;
        product.Price = request.Price;
        product.DiscountPercentage = request.DiscountPercentage;
        product.StockQuantity = request.StockQuantity;
        product.CategoryId = request.CategoryId;
        product.BrandId = request.BrandId;
        product.MainImageUrl = request.MainImageUrl;
        product.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return Result<ProductResponse>.Success(
            MapToResponse(product, category.Name, brand.Name, product.Seller.FullName));
    }

    public async Task<Result> Delete(Guid id)
    {
        var product = await _context.Products
            .FirstOrDefaultAsync(p => p.Id == id && !p.IsDeleted);

        if (product == null)
            return Result.Failure("Product not found");

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

        return Result.Success();
    }

    public async Task<Result<ProductResponse>> GetById(Guid id)
    {
        var product = await _context.Products
            .Include(p => p.Category)
            .Include(p => p.Brand)
            .Include(p => p.Seller)
            .FirstOrDefaultAsync(p => p.Id == id && !p.IsDeleted);

        if (product == null)
            return Result<ProductResponse>.Failure("Product not found");

        return Result<ProductResponse>.Success(
            MapToResponse(product, product.Category.Name, product.Brand.Name, product.Seller.FullName));
    }

    public async Task<Result<PagedResponse<ProductResponse>>> GetAll(ProductFilterRequest filter)
    {
        var query = _context.Products
            .Include(p => p.Category)
            .Include(p => p.Brand)
            .Include(p => p.Seller)
            .Where(p => !p.IsDeleted && p.IsActive);

        if (!string.IsNullOrWhiteSpace(filter.SearchTerm))
            query = query.Where(p => p.Name.Contains(filter.SearchTerm));

        if (filter.CategoryId.HasValue)
            query = query.Where(p => p.CategoryId == filter.CategoryId.Value);

        if (filter.BrandId.HasValue)
            query = query.Where(p => p.BrandId == filter.BrandId.Value);

        var totalCount = await query.CountAsync();
        var totalPages = (int)Math.Ceiling(totalCount / (double)filter.PageSize);

        var products = await query
            .OrderByDescending(p => p.CreatedAt)
            .Skip((filter.PageNumber - 1) * filter.PageSize)
            .Take(filter.PageSize)
            .ToListAsync();

        var items = products.Select(p =>
            MapToResponse(p, p.Category.Name, p.Brand.Name, p.Seller.FullName)).ToList();

        return Result<PagedResponse<ProductResponse>>.Success(new PagedResponse<ProductResponse>
        {
            Items = items,
            PageNumber = filter.PageNumber,
            PageSize = filter.PageSize,
            TotalCount = totalCount,
            TotalPages = totalPages
        });
    }

    private static ProductResponse MapToResponse(Product product, string categoryName, string brandName, string sellerName)
    {
        return new ProductResponse
        {
            Id = product.Id,
            Name = product.Name,
            Description = product.Description,
            Price = product.Price,
            DiscountPercentage = product.DiscountPercentage,
            FinalPrice = product.Price - (product.Price * product.DiscountPercentage / 100),
            StockQuantity = product.StockQuantity,
            MainImageUrl = product.MainImageUrl,
            CategoryName = categoryName,
            BrandName = brandName,
            SellerName = sellerName,
            CreatedAt = product.CreatedAt
        };
    }
}
