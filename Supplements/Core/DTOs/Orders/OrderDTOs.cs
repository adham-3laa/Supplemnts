using System.ComponentModel.DataAnnotations;

namespace Supplements.Core.DTOs.Orders;

public class CreateOrderRequest
{
    [Required] public Guid AddressId { get; set; }
    public string? Notes { get; set; }
}

public class OrderItemResponse
{
    public Guid Id { get; set; }
    public string ProductName { get; set; } = null!;
    public string? Flavor { get; set; }
    public string? Size { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal TotalPrice { get; set; }
}

public class OrderResponse
{
    public Guid Id { get; set; }
    public DateTime OrderDate { get; set; }
    public string Status { get; set; } = null!;
    public decimal TotalPrice { get; set; }
    public decimal ShippingCost { get; set; }
    public string? Notes { get; set; }
    public string Address { get; set; } = null!;
    public List<OrderItemResponse> Items { get; set; } = new();
}
