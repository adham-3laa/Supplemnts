using Supplements.ViewModels.Shared;

namespace Supplements.ViewModels.Shop;

public class ProductListViewModel
{
    public PagedViewModel<ProductCardViewModel> Products { get; set; } = new();
    public List<CategoryViewModel> Categories { get; set; } = new();
    public List<BrandViewModel> Brands { get; set; } = new();
    public string? SearchTerm { get; set; }
    public int? CategoryId { get; set; }
    public int? BrandId { get; set; }
    public string SortBy { get; set; } = "newest";
    public string? SelectedCategoryName { get; set; }
    public string? SelectedBrandName { get; set; }
}

public class ProductCardViewModel
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public decimal DiscountPercentage { get; set; }
    public decimal FinalPrice { get; set; }
    public int StockQuantity { get; set; }
    public string? MainImageUrl { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public string BrandName { get; set; } = string.Empty;
    public string SellerName { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public bool InStock => StockQuantity > 0;
    public bool HasDiscount => DiscountPercentage > 0;
}

public class CategoryViewModel
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int ProductCount { get; set; }
}

public class BrandViewModel
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? LogoUrl { get; set; }
    public int ProductCount { get; set; }
}
