using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Supplements.Core.DTOs.Orders;
using Supplements.Infrastructure.Data;
using Supplements.Services;
using Supplements.ViewModels.Checkout;

namespace Supplements.Controllers;

[Authorize]
public class CheckoutController : Controller
{
    private readonly CartService _cartService;
    private readonly OrderService _orderService;
    private readonly AppDbContext _context;

    public CheckoutController(CartService cartService, OrderService orderService, AppDbContext context)
    {
        _cartService = cartService;
        _orderService = orderService;
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var cartResult = await _cartService.GetCart(userId);

        if (!cartResult.IsSuccess || !cartResult.Data!.Items.Any())
        {
            TempData["Error"] = "Your cart is empty";
            return RedirectToAction("Index", "Cart");
        }

        var cart = cartResult.Data!;
        var addresses = await _context.Addresses
            .Where(a => a.UserId == userId && !a.IsDeleted)
            .OrderByDescending(a => a.IsDefault)
            .ToListAsync();

        var viewModel = new CheckoutViewModel
        {
            Items = cart.Items.Select(i => new ViewModels.Cart.CartItemViewModel
            {
                Id = i.Id,
                ProductVariantId = i.ProductVariantId,
                ProductName = i.ProductName,
                Flavor = i.Flavor,
                Size = i.Size,
                UnitPrice = i.UnitPrice,
                Quantity = i.Quantity,
                TotalPrice = i.TotalPrice
            }).ToList(),
            SubTotal = cart.TotalPrice,
            TotalPrice = cart.TotalPrice + 10,
            Addresses = addresses.Select(a => new AddressViewModel
            {
                Id = a.Id,
                Country = a.Country,
                City = a.City,
                Street = a.Street,
                BuildingNumber = a.BuildingNumber,
                Floor = a.Floor,
                Notes = a.Notes,
                IsDefault = a.IsDefault
            }).ToList(),
            SelectedAddressId = addresses.FirstOrDefault()?.Id
        };

        return View(viewModel);
    }

    [HttpPost]
    public async Task<IActionResult> PlaceOrder(CheckoutViewModel model)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        if (!model.SelectedAddressId.HasValue)
        {
            TempData["Error"] = "Please select a shipping address";
            return RedirectToAction("Index");
        }

        var result = await _orderService.CreateFromCart(userId, new CreateOrderRequest
        {
            AddressId = model.SelectedAddressId.Value,
            Notes = model.Notes
        });

        if (!result.IsSuccess)
        {
            TempData["Error"] = result.Message;
            return RedirectToAction("Index");
        }

        TempData["Success"] = "Order placed successfully!";
        return RedirectToAction("Details", "Order", new { id = result.Data!.Id });
    }
}
