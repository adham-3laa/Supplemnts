using Microsoft.EntityFrameworkCore;
using Supplements.Core;
using Supplements.Core.DTOs.Cart;
using Supplements.Core.Entities;
using Supplements.Infrastructure.Data;

namespace Supplements.Services;

public class CartService
{
    private readonly AppDbContext _context;

    public CartService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Result<CartResponse>> AddToCart(Guid userId, AddToCartRequest request)
    {
        var variant = await _context.ProductVariants
            .Include(v => v.Product)
            .FirstOrDefaultAsync(v => v.Id == request.ProductVariantId && !v.IsDeleted);

        if (variant == null)
            return Result<CartResponse>.Failure("Product variant not found");

        if (variant.StockQuantity < request.Quantity)
            return Result<CartResponse>.Failure("Not enough stock available");

        var cart = await _context.Carts
            .Include(c => c.Items)
            .FirstOrDefaultAsync(c => c.UserId == userId);

        if (cart == null)
        {
            cart = new Cart { Id = Guid.NewGuid(), UserId = userId };
            _context.Carts.Add(cart);
        }

        var existingItem = cart.Items
            .FirstOrDefault(i => i.ProductVariantId == request.ProductVariantId);

        if (existingItem != null)
        {
            existingItem.Quantity += request.Quantity;
        }
        else
        {
            cart.Items.Add(new CartItem
            {
                Id = Guid.NewGuid(),
                CartId = cart.Id,
                ProductVariantId = request.ProductVariantId,
                Quantity = request.Quantity
            });
        }

        await _context.SaveChangesAsync();
        return Result<CartResponse>.Success(await BuildCartResponse(cart.Id));
    }

    public async Task<Result> RemoveFromCart(Guid userId, Guid cartItemId)
    {
        var cart = await _context.Carts
            .Include(c => c.Items)
            .FirstOrDefaultAsync(c => c.UserId == userId);

        var item = cart?.Items.FirstOrDefault(i => i.Id == cartItemId);
        if (item == null)
            return Result.Failure("Cart item not found");

        _context.CartItems.Remove(item);
        await _context.SaveChangesAsync();
        return Result.Success();
    }

    public async Task<Result> UpdateQuantity(Guid userId, Guid cartItemId, int quantity)
    {
        if (quantity <= 0)
            return Result.Failure("Quantity must be greater than zero");

        var cart = await _context.Carts
            .Include(c => c.Items)
            .FirstOrDefaultAsync(c => c.UserId == userId);

        var item = cart?.Items.FirstOrDefault(i => i.Id == cartItemId);
        if (item == null)
            return Result.Failure("Cart item not found");

        var variant = await _context.ProductVariants.FindAsync(item.ProductVariantId);
        if (variant == null)
            return Result.Failure("Product variant not found");

        if (variant.StockQuantity < quantity)
            return Result.Failure("Not enough stock available");

        item.Quantity = quantity;
        await _context.SaveChangesAsync();
        return Result.Success();
    }

    public async Task<Result<CartResponse>> GetCart(Guid userId)
    {
        var cart = await _context.Carts
            .Include(c => c.Items)
            .ThenInclude(i => i.ProductVariant)
            .ThenInclude(v => v.Product)
            .FirstOrDefaultAsync(c => c.UserId == userId);

        if (cart == null)
            return Result<CartResponse>.Failure("Cart not found");

        return Result<CartResponse>.Success(new CartResponse
        {
            CartId = cart.Id,
            Items = cart.Items.Select(i => new CartItemResponse
            {
                Id = i.Id,
                ProductVariantId = i.ProductVariantId,
                ProductName = i.ProductVariant.Product.Name,
                Flavor = i.ProductVariant.Flavor,
                Size = i.ProductVariant.Size,
                UnitPrice = i.ProductVariant.Product.Price + i.ProductVariant.AdditionalPrice,
                Quantity = i.Quantity,
                TotalPrice = (i.ProductVariant.Product.Price + i.ProductVariant.AdditionalPrice) * i.Quantity
            }).ToList(),
            TotalPrice = cart.Items.Sum(i =>
                (i.ProductVariant.Product.Price + i.ProductVariant.AdditionalPrice) * i.Quantity)
        });
    }

    private async Task<CartResponse> BuildCartResponse(Guid cartId)
    {
        var cart = await _context.Carts
            .Include(c => c.Items)
            .ThenInclude(i => i.ProductVariant)
            .ThenInclude(v => v.Product)
            .FirstOrDefaultAsync(c => c.Id == cartId);

        return new CartResponse
        {
            CartId = cart!.Id,
            Items = cart.Items.Select(i => new CartItemResponse
            {
                Id = i.Id,
                ProductVariantId = i.ProductVariantId,
                ProductName = i.ProductVariant.Product.Name,
                Flavor = i.ProductVariant.Flavor,
                Size = i.ProductVariant.Size,
                UnitPrice = i.ProductVariant.Product.Price + i.ProductVariant.AdditionalPrice,
                Quantity = i.Quantity,
                TotalPrice = (i.ProductVariant.Product.Price + i.ProductVariant.AdditionalPrice) * i.Quantity
            }).ToList(),
            TotalPrice = cart.Items.Sum(i =>
                (i.ProductVariant.Product.Price + i.ProductVariant.AdditionalPrice) * i.Quantity)
        };
    }
}
