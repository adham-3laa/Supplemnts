using Microsoft.EntityFrameworkCore;
using Supplements.Core;
using Supplements.Core.DTOs.Orders;
using Supplements.Core.Entities;
using Supplements.Infrastructure.Data;

namespace Supplements.Services;

public class OrderService
{
    private readonly AppDbContext _context;

    public OrderService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Result<OrderResponse>> CreateFromCart(Guid userId, CreateOrderRequest request)
    {
        var cart = await _context.Carts
            .Include(c => c.Items)
            .ThenInclude(i => i.ProductVariant)
            .ThenInclude(v => v.Product)
            .FirstOrDefaultAsync(c => c.UserId == userId);

        if (cart == null || !cart.Items.Any())
            return Result<OrderResponse>.Failure("Cart is empty");

        var address = await _context.Addresses
            .FirstOrDefaultAsync(a => a.Id == request.AddressId && a.UserId == userId && !a.IsDeleted);

        if (address == null)
            return Result<OrderResponse>.Failure("Address not found");

        foreach (var item in cart.Items)
        {
            if (item.ProductVariant.StockQuantity < item.Quantity)
                return Result<OrderResponse>.Failure(
                    $"Insufficient stock for {item.ProductVariant.Product.Name}");
        }

        var order = new Order
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            AddressId = request.AddressId,
            Status = "Pending",
            ShippingCost = 10,
            Notes = request.Notes
        };

        foreach (var cartItem in cart.Items)
        {
            var unitPrice = cartItem.ProductVariant.Product.Price + cartItem.ProductVariant.AdditionalPrice;
            var discount = cartItem.ProductVariant.Product.DiscountPercentage;

            order.Items.Add(new OrderItem
            {
                Id = Guid.NewGuid(),
                OrderId = order.Id,
                ProductVariantId = cartItem.ProductVariantId,
                Quantity = cartItem.Quantity,
                UnitPrice = unitPrice,
                DiscountPercentage = discount,
                TotalPrice = (unitPrice - unitPrice * discount / 100) * cartItem.Quantity
            });

            cartItem.ProductVariant.StockQuantity -= cartItem.Quantity;
        }

        order.TotalPrice = order.Items.Sum(i => i.TotalPrice) + order.ShippingCost;

        _context.Orders.Add(order);
        _context.CartItems.RemoveRange(cart.Items);
        await _context.SaveChangesAsync();

        return Result<OrderResponse>.Success(await BuildOrderResponse(order.Id));
    }

    public async Task<Result<List<OrderResponse>>> GetUserOrders(Guid userId)
    {
        var orders = await _context.Orders
            .Include(o => o.Items)
            .ThenInclude(i => i.ProductVariant)
            .ThenInclude(v => v.Product)
            .Include(o => o.Address)
            .Where(o => o.UserId == userId && !o.IsDeleted)
            .OrderByDescending(o => o.OrderDate)
            .ToListAsync();

        var result = orders.Select(o => new OrderResponse
        {
            Id = o.Id,
            OrderDate = o.OrderDate,
            Status = o.Status,
            TotalPrice = o.TotalPrice,
            ShippingCost = o.ShippingCost,
            Notes = o.Notes,
            Address = $"{o.Address.Street}, {o.Address.City}, {o.Address.Country}",
            Items = o.Items.Select(i => new OrderItemResponse
            {
                Id = i.Id,
                ProductName = i.ProductVariant.Product.Name,
                Flavor = i.ProductVariant.Flavor,
                Size = i.ProductVariant.Size,
                Quantity = i.Quantity,
                UnitPrice = i.UnitPrice,
                TotalPrice = i.TotalPrice
            }).ToList()
        }).ToList();

        return Result<List<OrderResponse>>.Success(result);
    }

    private async Task<OrderResponse> BuildOrderResponse(Guid orderId)
    {
        var order = await _context.Orders
            .Include(o => o.Items)
            .ThenInclude(i => i.ProductVariant)
            .ThenInclude(v => v.Product)
            .Include(o => o.Address)
            .FirstOrDefaultAsync(o => o.Id == orderId);

        return new OrderResponse
        {
            Id = order!.Id,
            OrderDate = order.OrderDate,
            Status = order.Status,
            TotalPrice = order.TotalPrice,
            ShippingCost = order.ShippingCost,
            Notes = order.Notes,
            Address = $"{order.Address.Street}, {order.Address.City}, {order.Address.Country}",
            Items = order.Items.Select(i => new OrderItemResponse
            {
                Id = i.Id,
                ProductName = i.ProductVariant.Product.Name,
                Flavor = i.ProductVariant.Flavor,
                Size = i.ProductVariant.Size,
                Quantity = i.Quantity,
                UnitPrice = i.UnitPrice,
                TotalPrice = i.TotalPrice
            }).ToList()
        };
    }
}
