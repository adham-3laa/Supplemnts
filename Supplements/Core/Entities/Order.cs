namespace Supplements.Core.Entities;
public class Order : BaseEntity
{
    public Guid UserId { get; set; }

    public User User { get; set; } = null!;

    public Guid AddressId { get; set; }

    public Address Address { get; set; } = null!;

    public DateTime OrderDate { get; set; } = DateTime.UtcNow;

    public string Status { get; set; } = null!;

    public decimal TotalPrice { get; set; }

    public decimal ShippingCost { get; set; }

    public string? Notes { get; set; }

    public ICollection<OrderItem> Items { get; set; } = new List<OrderItem>();
}
