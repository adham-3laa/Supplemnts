namespace Supplements.ViewModels.Order;

public class OrderListViewModel
{
    public List<OrderSummaryViewModel> Orders { get; set; } = new();
}

public class OrderSummaryViewModel
{
    public Guid Id { get; set; }
    public DateTime OrderDate { get; set; }
    public string Status { get; set; } = string.Empty;
    public decimal TotalPrice { get; set; }
    public int ItemCount { get; set; }
    public string StatusColor => Status switch
    {
        "Pending" => "yellow",
        "Processing" => "blue",
        "Shipped" => "purple",
        "Delivered" => "green",
        "Cancelled" => "red",
        _ => "gray"
    };
}

public class OrderDetailViewModel
{
    public Guid Id { get; set; }
    public DateTime OrderDate { get; set; }
    public string Status { get; set; } = string.Empty;
    public decimal TotalPrice { get; set; }
    public decimal ShippingCost { get; set; }
    public decimal SubTotal => TotalPrice - ShippingCost;
    public string? Notes { get; set; }
    public string Address { get; set; } = string.Empty;
    public List<OrderItemViewModel> Items { get; set; } = new();
}

public class OrderItemViewModel
{
    public Guid Id { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string? Flavor { get; set; }
    public string? Size { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal TotalPrice { get; set; }
    public string? ImageUrl { get; set; }
}
