using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Supplements.Services;
using Supplements.ViewModels.Cart;

namespace Supplements.Controllers;

[Authorize]
public class CartController : Controller
{
    private readonly CartService _cartService;

    public CartController(CartService cartService)
    {
        _cartService = cartService;
    }

    public async Task<IActionResult> Index()
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var result = await _cartService.GetCart(userId);

        if (!result.IsSuccess)
        {
            return View(new CartViewModel());
        }

        var cart = result.Data!;
        var viewModel = new CartViewModel
        {
            CartId = cart.CartId,
            Items = cart.Items.Select(i => new CartItemViewModel
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
            TotalPrice = cart.TotalPrice
        };

        return View(viewModel);
    }

    [HttpPost]
    public async Task<IActionResult> AddToCart(Core.DTOs.Cart.AddToCartRequest request)
    {
        if (!ModelState.IsValid)
        {
            TempData["Error"] = "Invalid request";
            return Redirect(Request.Headers["Referer"].ToString());
        }

        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var result = await _cartService.AddToCart(userId, request);

        if (!result.IsSuccess)
        {
            TempData["Error"] = result.Message;
            return Redirect(Request.Headers["Referer"].ToString());
        }

        TempData["Success"] = "Item added to cart";
        return Redirect(Request.Headers["Referer"].ToString());
    }

    [HttpPost]
    public async Task<IActionResult> UpdateQuantity(Guid cartItemId, int quantity)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var result = await _cartService.UpdateQuantity(userId, cartItemId, quantity);

        if (!result.IsSuccess)
            TempData["Error"] = result.Message;
        else
            TempData["Success"] = "Cart updated";

        return RedirectToAction("Index");
    }

    [HttpPost]
    public async Task<IActionResult> RemoveFromCart(Guid cartItemId)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var result = await _cartService.RemoveFromCart(userId, cartItemId);

        if (!result.IsSuccess)
            TempData["Error"] = result.Message;
        else
            TempData["Success"] = "Item removed from cart";

        return RedirectToAction("Index");
    }
}
