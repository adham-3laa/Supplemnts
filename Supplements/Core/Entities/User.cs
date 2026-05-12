using Microsoft.AspNetCore.Identity;

namespace Supplements.Core.Entities;

public class User : IdentityUser<Guid>
{
    public string FullName { get; set; } = null!;

    public bool IsActive { get; set; } = true;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? UpdatedAt { get; set; }

    public bool IsDeleted { get; set; } = false;

    public ICollection<Address> Addresses { get; set; } = new List<Address>();

    public ICollection<Product> Products { get; set; } = new List<Product>();

    public ICollection<Order> Orders { get; set; } = new List<Order>();

    public Cart? Cart { get; set; }

    public Wishlist? Wishlist { get; set; }
}
