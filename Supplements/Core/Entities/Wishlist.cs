namespace Supplements.Core.Entities;
public class Wishlist : BaseEntity
{
    public Guid UserId { get; set; }

    public User User { get; set; } = null!;

    public ICollection<WishlistItem> Items { get; set; } = new List<WishlistItem>();
}
