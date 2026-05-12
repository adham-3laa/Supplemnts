using Microsoft.EntityFrameworkCore;
using Supplements.Core;
using Supplements.Core.DTOs;
using Supplements.Core.Entities;
using Supplements.Infrastructure.Data;

namespace Supplements.Services;

public class WishlistService
{
    private readonly AppDbContext _context;

    public WishlistService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Result<WishlistResponse>> AddToWishlist(Guid userId, Guid productId)
    {
        var product = await _context.Products
            .FirstOrDefaultAsync(p => p.Id == productId && !p.IsDeleted);

        if (product == null)
            return Result<WishlistResponse>.Failure("Product not found");

        var wishlist = await _context.Wishlists
            .Include(w => w.Items)
            .FirstOrDefaultAsync(w => w.UserId == userId);

        if (wishlist == null)
        {
            wishlist = new Wishlist { Id = Guid.NewGuid(), UserId = userId };
            _context.Wishlists.Add(wishlist);
        }

        if (wishlist.Items.Any(i => i.ProductId == productId))
            return Result<WishlistResponse>.Failure("Product already in wishlist");

        wishlist.Items.Add(new WishlistItem
        {
            Id = Guid.NewGuid(),
            WishlistId = wishlist.Id,
            ProductId = productId
        });

        await _context.SaveChangesAsync();
        return Result<WishlistResponse>.Success(await BuildWishlistResponse(wishlist.Id));
    }

    public async Task<Result> RemoveFromWishlist(Guid userId, Guid wishlistItemId)
    {
        var wishlist = await _context.Wishlists
            .Include(w => w.Items)
            .FirstOrDefaultAsync(w => w.UserId == userId);

        var item = wishlist?.Items.FirstOrDefault(i => i.Id == wishlistItemId);
        if (item == null)
            return Result.Failure("Wishlist item not found");

        _context.WishlistItems.Remove(item);
        await _context.SaveChangesAsync();
        return Result.Success();
    }

    public async Task<Result<WishlistResponse>> GetWishlist(Guid userId)
    {
        var wishlist = await _context.Wishlists
            .Include(w => w.Items)
            .ThenInclude(i => i.Product)
            .FirstOrDefaultAsync(w => w.UserId == userId);

        if (wishlist == null)
            return Result<WishlistResponse>.Failure("Wishlist not found");

        return Result<WishlistResponse>.Success(new WishlistResponse
        {
            WishlistId = wishlist.Id,
            Items = wishlist.Items.Select(i => new WishlistItemResponse
            {
                Id = i.Id,
                ProductId = i.ProductId,
                ProductName = i.Product.Name,
                Price = i.Product.Price,
                MainImageUrl = i.Product.MainImageUrl
            }).ToList()
        });
    }

    private async Task<WishlistResponse> BuildWishlistResponse(Guid wishlistId)
    {
        var wishlist = await _context.Wishlists
            .Include(w => w.Items)
            .ThenInclude(i => i.Product)
            .FirstOrDefaultAsync(w => w.Id == wishlistId);

        return new WishlistResponse
        {
            WishlistId = wishlist!.Id,
            Items = wishlist.Items.Select(i => new WishlistItemResponse
            {
                Id = i.Id,
                ProductId = i.ProductId,
                ProductName = i.Product.Name,
                Price = i.Product.Price,
                MainImageUrl = i.Product.MainImageUrl
            }).ToList()
        };
    }
}
