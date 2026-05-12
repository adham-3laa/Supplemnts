namespace Supplements.Core.Entities;
public class OrderItem : BaseEntity
{
    public Guid OrderId { get; set; }

    public Order Order { get; set; } = null!;

    public Guid ProductVariantId { get; set; }

    public ProductVariant ProductVariant { get; set; } = null!;

    public int Quantity { get; set; }

    public decimal UnitPrice { get; set; }

    public decimal DiscountPercentage { get; set; }

    public decimal TotalPrice { get; set; }
}
