using System.ComponentModel.DataAnnotations;

namespace Supplements.Core.DTOs;

public class RegisterRequest
{
    [Required] public string FullName { get; set; } = null!;
    [Required] [EmailAddress] public string Email { get; set; } = null!;
    [Required] [MinLength(8)] public string Password { get; set; } = null!;
    public string? PhoneNumber { get; set; }
}

public class LoginRequest
{
    [Required] [EmailAddress] public string Email { get; set; } = null!;
    [Required] public string Password { get; set; } = null!;
}

public class AuthResponse
{
    public string Token { get; set; } = null!;
    public string FullName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public Guid UserId { get; set; }
}
