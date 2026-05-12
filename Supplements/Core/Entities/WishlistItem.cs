namespace Supplements.Core.Entities;
public class WishlistItem : BaseEntity
{
    public Guid WishlistId { get; set; }

    public Wishlist Wishlist { get; set; } = null!;

    public Guid ProductId { get; set; }

    public Product Product { get; set; } = null!;
}
