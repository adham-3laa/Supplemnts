namespace Supplements.ViewModels.Cart;

public class CartViewModel
{
    public Guid CartId { get; set; }
    public List<CartItemViewModel> Items { get; set; } = new();
    public decimal TotalPrice { get; set; }
    public int ItemCount => Items.Sum(i => i.Quantity);
    public bool IsEmpty => !Items.Any();
}

public class CartItemViewModel
{
    public Guid Id { get; set; }
    public Guid ProductVariantId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string? Flavor { get; set; }
    public string? Size { get; set; }
    public string? ImageUrl { get; set; }
    public decimal UnitPrice { get; set; }
    public int Quantity { get; set; }
    public decimal TotalPrice { get; set; }
}
