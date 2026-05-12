using Microsoft.AspNetCore.Mvc;
using Supplements.Core.DTOs;

namespace Supplements.Services;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly AuthService _authService;

    public AuthController(AuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        var result = await _authService.Register(request);
        if (!result.IsSuccess)
            return BadRequest(new { message = result.Message });

        return Ok(result.Data);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var result = await _authService.Login(request);
        if (!result.IsSuccess)
            return Unauthorized(new { message = result.Message });

        return Ok(result.Data);
    }
}
