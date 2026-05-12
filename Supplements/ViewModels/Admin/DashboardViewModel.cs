namespace Supplements.ViewModels.Admin;

public class DashboardViewModel
{
    public int TotalProducts { get; set; }
    public int TotalOrders { get; set; }
    public int TotalUsers { get; set; }
    public int TotalCategories { get; set; }
    public int TotalBrands { get; set; }
    public int LowStockProducts { get; set; }
    public decimal TotalRevenue { get; set; }
    public List<RecentOrderViewModel> RecentOrders { get; set; } = new();
    public List<TopProductViewModel> TopProducts { get; set; } = new();
}

public class RecentOrderViewModel
{
    public Guid Id { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public DateTime OrderDate { get; set; }
    public decimal TotalPrice { get; set; }
    public string Status { get; set; } = string.Empty;
}

public class TopProductViewModel
{
    public string Name { get; set; } = string.Empty;
    public int TotalSold { get; set; }
    public decimal Revenue { get; set; }
}

public class ProductManageViewModel
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int StockQuantity { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public string BrandName { get; set; } = string.Empty;
    public string SellerName { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class UserManageViewModel
{
    public Guid Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public IList<string> Roles { get; set; } = new List<string>();
}

public class SalesSummaryViewModel
{
    public decimal TotalRevenue { get; set; }
    public int TotalOrders { get; set; }
    public decimal AverageOrderValue { get; set; }
    public int TotalProductsSold { get; set; }
    public List<MonthlySalesViewModel> MonthlySales { get; set; } = new();
}

public class MonthlySalesViewModel
{
    public string Month { get; set; } = string.Empty;
    public int Orders { get; set; }
    public decimal Revenue { get; set; }
}
