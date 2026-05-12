namespace Supplements.ViewModels.Shop;

public class ProductDetailViewModel
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public decimal DiscountPercentage { get; set; }
    public decimal FinalPrice { get; set; }
    public decimal SavedAmount => Price - FinalPrice;
    public int StockQuantity { get; set; }
    public string? MainImageUrl { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public int CategoryId { get; set; }
    public string BrandName { get; set; } = string.Empty;
    public int BrandId { get; set; }
    public string SellerName { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public bool InStock => StockQuantity > 0;
    public bool HasDiscount => DiscountPercentage > 0;
    public List<ProductImageViewModel> Images { get; set; } = new();
    public List<ProductVariantViewModel> Variants { get; set; } = new();
}

public class ProductImageViewModel
{
    public Guid Id { get; set; }
    public string ImageUrl { get; set; } = string.Empty;
}

public class ProductVariantViewModel
{
    public Guid Id { get; set; }
    public string? Flavor { get; set; }
    public string? Size { get; set; }
    public decimal AdditionalPrice { get; set; }
    public int StockQuantity { get; set; }
    public bool InStock => StockQuantity > 0;
}
