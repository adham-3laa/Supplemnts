using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Supplements.Core.DTOs.Orders;

namespace Supplements.Services;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class OrdersController : ControllerBase
{
    private readonly OrderService _orderService;

    public OrdersController(OrderService orderService)
    {
        _orderService = orderService;
    }

    [HttpPost]
    public async Task<IActionResult> CreateOrder([FromBody] CreateOrderRequest request)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var result = await _orderService.CreateFromCart(userId, request);
        if (!result.IsSuccess)
            return BadRequest(new { message = result.Message });

        return Ok(result.Data);
    }

    [HttpGet]
    public async Task<IActionResult> GetOrders()
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var result = await _orderService.GetUserOrders(userId);
        return Ok(result.Data);
    }
}
