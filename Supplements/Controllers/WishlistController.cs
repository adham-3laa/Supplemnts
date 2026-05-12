using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Supplements.Services;
using Supplements.ViewModels.Wishlist;

namespace Supplements.Controllers;

[Authorize]
public class WishlistController : Controller
{
    private readonly WishlistService _wishlistService;

    public WishlistController(WishlistService wishlistService)
    {
        _wishlistService = wishlistService;
    }

    public async Task<IActionResult> Index()
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var result = await _wishlistService.GetWishlist(userId);

        if (!result.IsSuccess)
            return View(new WishlistViewModel());

        var wishlist = result.Data!;
        var viewModel = new WishlistViewModel
        {
            WishlistId = wishlist.WishlistId,
            Items = wishlist.Items.Select(i => new WishlistItemViewModel
            {
                Id = i.Id,
                ProductId = i.ProductId,
                ProductName = i.ProductName,
                Price = i.Price,
                MainImageUrl = i.MainImageUrl
            }).ToList()
        };

        return View(viewModel);
    }

    [HttpPost]
    public async Task<IActionResult> AddToWishlist(Guid productId)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var result = await _wishlistService.AddToWishlist(userId, productId);

        if (!result.IsSuccess)
            TempData["Error"] = result.Message;
        else
            TempData["Success"] = "Added to wishlist";

        return Redirect(Request.Headers["Referer"].ToString());
    }

    [HttpPost]
    public async Task<IActionResult> RemoveFromWishlist(Guid wishlistItemId)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var result = await _wishlistService.RemoveFromWishlist(userId, wishlistItemId);

        if (!result.IsSuccess)
            TempData["Error"] = result.Message;
        else
            TempData["Success"] = "Removed from wishlist";

        return RedirectToAction("Index");
    }
}
