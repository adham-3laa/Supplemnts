using System.ComponentModel.DataAnnotations;

namespace Supplements.Core.DTOs.Cart;

public class AddToCartRequest
{
    [Required] public Guid ProductVariantId { get; set; }
    [Required] [Range(1, int.MaxValue)] public int Quantity { get; set; }
}

public class UpdateCartItemRequest
{
    [Required] [Range(1, int.MaxValue)] public int Quantity { get; set; }
}

public class CartItemResponse
{
    public Guid Id { get; set; }
    public Guid ProductVariantId { get; set; }
    public string ProductName { get; set; } = null!;
    public string? Flavor { get; set; }
    public string? Size { get; set; }
    public decimal UnitPrice { get; set; }
    public int Quantity { get; set; }
    public decimal TotalPrice { get; set; }
}

public class CartResponse
{
    public Guid CartId { get; set; }
    public List<CartItemResponse> Items { get; set; } = new();
    public decimal TotalPrice { get; set; }
}
