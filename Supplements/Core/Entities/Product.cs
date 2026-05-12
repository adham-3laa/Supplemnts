using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace Supplements.Core.Entities;
public class Product : BaseEntity
{
    public Guid SellerId { get; set; }

    [ValidateNever]
    public User Seller { get; set; } = null!;

    public int CategoryId { get; set; }

    [ValidateNever]
    public Category Category { get; set; } = null!;

    public int BrandId { get; set; }

    [ValidateNever]
    public Brand Brand { get; set; } = null!;

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public decimal Price { get; set; }

    public decimal DiscountPercentage { get; set; }

    public int StockQuantity { get; set; }

    public string? MainImageUrl { get; set; }

    public bool IsActive { get; set; } = true;

    public ICollection<ProductImage> Images { get; set; } = new List<ProductImage>();

    public ICollection<ProductVariant> Variants { get; set; } = new List<ProductVariant>();

    public ICollection<WishlistItem> WishlistItems { get; set; } = new List<WishlistItem>();
}
