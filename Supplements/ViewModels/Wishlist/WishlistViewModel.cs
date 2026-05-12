namespace Supplements.ViewModels.Wishlist;

public class WishlistViewModel
{
    public Guid WishlistId { get; set; }
    public List<WishlistItemViewModel> Items { get; set; } = new();
    public bool IsEmpty => !Items.Any();
}

public class WishlistItemViewModel
{
    public Guid Id { get; set; }
    public Guid ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public decimal DiscountPercentage { get; set; }
    public decimal FinalPrice { get; set; }
    public string? MainImageUrl { get; set; }
    public bool InStock { get; set; }
    public bool HasDiscount => DiscountPercentage > 0;
}
