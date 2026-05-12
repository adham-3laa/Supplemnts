namespace Supplements.Core.Entities;
public class ProductVariant : BaseEntity
{
    public Guid ProductId { get; set; }

    public Product Product { get; set; } = null!;

    public string? Flavor { get; set; }

    public string? Size { get; set; }

    public decimal AdditionalPrice { get; set; }

    public int StockQuantity { get; set; }

    public ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();

    public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
}
