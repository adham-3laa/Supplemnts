using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Supplements.Services;
using Supplements.ViewModels.Order;

namespace Supplements.Controllers;

[Authorize]
public class OrderController : Controller
{
    private readonly OrderService _orderService;

    public OrderController(OrderService orderService)
    {
        _orderService = orderService;
    }

    public async Task<IActionResult> Index()
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var result = await _orderService.GetUserOrders(userId);

        var viewModel = new OrderListViewModel
        {
            Orders = (result.Data ?? new List<Core.DTOs.Orders.OrderResponse>()).Select(o => new OrderSummaryViewModel
            {
                Id = o.Id,
                OrderDate = o.OrderDate,
                Status = o.Status,
                TotalPrice = o.TotalPrice,
                ItemCount = o.Items.Count
            }).ToList()
        };

        return View(viewModel);
    }

    public async Task<IActionResult> Details(Guid id)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var result = await _orderService.GetUserOrders(userId);

        var order = result.Data?.FirstOrDefault(o => o.Id == id);
        if (order == null) return NotFound();

        var viewModel = new OrderDetailViewModel
        {
            Id = order.Id,
            OrderDate = order.OrderDate,
            Status = order.Status,
            TotalPrice = order.TotalPrice,
            ShippingCost = order.ShippingCost,
            Notes = order.Notes,
            Address = order.Address,
            Items = order.Items.Select(i => new OrderItemViewModel
            {
                Id = i.Id,
                ProductName = i.ProductName,
                Flavor = i.Flavor,
                Size = i.Size,
                Quantity = i.Quantity,
                UnitPrice = i.UnitPrice,
                TotalPrice = i.TotalPrice
            }).ToList()
        };

        return View(viewModel);
    }
}
