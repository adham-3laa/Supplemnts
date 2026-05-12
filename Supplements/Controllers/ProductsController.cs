using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Supplements.Core.DTOs;

namespace Supplements.Services;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly ProductService _productService;

    public ProductsController(ProductService productService)
    {
        _productService = productService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] ProductFilterRequest filter)
    {
        var result = await _productService.GetAll(filter);
        return Ok(result.Data);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await _productService.GetById(id);
        if (!result.IsSuccess)
            return NotFound(new { message = result.Message });

        return Ok(result.Data);
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Create([FromBody] CreateProductRequest request)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var result = await _productService.Create(userId, request);
        if (!result.IsSuccess)
            return BadRequest(new { message = result.Message });

        return Ok(result.Data);
    }

    [HttpPut("{id}")]
    [Authorize]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateProductRequest request)
    {
        var result = await _productService.Update(id, request);
        if (!result.IsSuccess)
            return BadRequest(new { message = result.Message });

        return Ok(result.Data);
    }

    [HttpDelete("{id}")]
    [Authorize]
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await _productService.Delete(id);
        if (!result.IsSuccess)
            return NotFound(new { message = result.Message });

        return Ok();
    }
}
