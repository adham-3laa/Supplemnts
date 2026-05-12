using System.ComponentModel.DataAnnotations;
using Supplements.ViewModels.Cart;

namespace Supplements.ViewModels.Checkout;

public class CheckoutViewModel
{
    public List<CartItemViewModel> Items { get; set; } = new();
    public decimal SubTotal { get; set; }
    public decimal ShippingCost { get; set; } = 10;
    public decimal TotalPrice { get; set; }
    public List<AddressViewModel> Addresses { get; set; } = new();
    public Guid? SelectedAddressId { get; set; }
    public string? Notes { get; set; }
}

public class AddressViewModel
{
    public Guid Id { get; set; }
    public string Country { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string Street { get; set; } = string.Empty;
    public string? BuildingNumber { get; set; }
    public string? Floor { get; set; }
    public string? Notes { get; set; }
    public bool IsDefault { get; set; }
    public string FullAddress => $"{Street}, {City}, {Country}";
}
