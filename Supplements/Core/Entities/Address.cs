namespace Supplements.Core.Entities;
public class Address : BaseEntity
{
    public Guid UserId { get; set; }

    public User User { get; set; } = null!;

    public string Country { get; set; } = null!;

    public string City { get; set; } = null!;

    public string Street { get; set; } = null!;

    public string? BuildingNumber { get; set; }

    public string? Floor { get; set; }

    public string? Notes { get; set; }

    public bool IsDefault { get; set; }

    public ICollection<Order> Orders { get; set; } = new List<Order>();
}
