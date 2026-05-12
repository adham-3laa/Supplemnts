namespace Supplements.Core.DTOs;

public class WishlistItemResponse
{
    public Guid Id { get; set; }
    public Guid ProductId { get; set; }
    public string ProductName { get; set; } = null!;
    public decimal Price { get; set; }
    public string? MainImageUrl { get; set; }
}

public class WishlistResponse
{
    public Guid WishlistId { get; set; }
    public List<WishlistItemResponse> Items { get; set; } = new();
}
