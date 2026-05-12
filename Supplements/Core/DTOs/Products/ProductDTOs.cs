using System.ComponentModel.DataAnnotations;

namespace Supplements.Core.DTOs;

public class CreateProductVariantDto
{
    public string? Flavor { get; set; }
    public string? Size { get; set; }
    public decimal AdditionalPrice { get; set; }
    public int StockQuantity { get; set; }
}

public class CreateProductRequest
{
    [Required] public string Name { get; set; } = null!;
    public string? Description { get; set; }
    [Required] [Range(0.01, double.MaxValue)] public decimal Price { get; set; }
    [Range(0, 100)] public decimal DiscountPercentage { get; set; }
    [Required] [Range(0, int.MaxValue)] public int StockQuantity { get; set; }
    [Required] public int CategoryId { get; set; }
    [Required] public int BrandId { get; set; }
    public string? MainImageUrl { get; set; }
    public List<CreateProductVariantDto> Variants { get; set; } = new();
}

public class UpdateProductRequest
{
    [Required] public string Name { get; set; } = null!;
    public string? Description { get; set; }
    [Required] [Range(0.01, double.MaxValue)] public decimal Price { get; set; }
    [Range(0, 100)] public decimal DiscountPercentage { get; set; }
    [Required] [Range(0, int.MaxValue)] public int StockQuantity { get; set; }
    [Required] public int CategoryId { get; set; }
    [Required] public int BrandId { get; set; }
    public string? MainImageUrl { get; set; }
}

public class ProductResponse
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public decimal DiscountPercentage { get; set; }
    public decimal FinalPrice { get; set; }
    public int StockQuantity { get; set; }
    public string? MainImageUrl { get; set; }
    public string CategoryName { get; set; } = null!;
    public string BrandName { get; set; } = null!;
    public string SellerName { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
}

public class ProductFilterRequest
{
    public string? SearchTerm { get; set; }
    public int? CategoryId { get; set; }
    public int? BrandId { get; set; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}
