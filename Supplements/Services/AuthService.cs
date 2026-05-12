using Microsoft.AspNetCore.Identity;
using Supplements.Core;
using Supplements.Core.DTOs;
using Supplements.Core.Entities;

namespace Supplements.Services;

public class AuthService
{
    private readonly UserManager<User> _userManager;
    private readonly JwtService _jwtService;

    public AuthService(UserManager<User> userManager, JwtService jwtService)
    {
        _userManager = userManager;
        _jwtService = jwtService;
    }

    public async Task<Result<AuthResponse>> Register(RegisterRequest request)
    {
        var existingUser = await _userManager.FindByEmailAsync(request.Email);
        if (existingUser != null)
            return Result<AuthResponse>.Failure("Email is already registered");

        var user = new User
        {
            Id = Guid.NewGuid(),
            UserName = request.Email,
            Email = request.Email,
            FullName = request.FullName,
            PhoneNumber = request.PhoneNumber
        };

        var result = await _userManager.CreateAsync(user, request.Password);
        if (!result.Succeeded)
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            return Result<AuthResponse>.Failure(errors);
        }

        var token = _jwtService.GenerateToken(user);
        return Result<AuthResponse>.Success(new AuthResponse
        {
            Token = token,
            FullName = user.FullName,
            Email = user.Email!,
            UserId = user.Id
        });
    }

    public async Task<Result<AuthResponse>> Login(LoginRequest request)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user == null || !await _userManager.CheckPasswordAsync(user, request.Password))
            return Result<AuthResponse>.Failure("Invalid email or password");

        if (!user.IsActive || user.IsDeleted)
            return Result<AuthResponse>.Failure("Account is inactive or deleted");

        var token = _jwtService.GenerateToken(user);
        return Result<AuthResponse>.Success(new AuthResponse
        {
            Token = token,
            FullName = user.FullName,
            Email = user.Email!,
            UserId = user.Id
        });
    }
}
