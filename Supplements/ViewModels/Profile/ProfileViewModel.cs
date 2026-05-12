using System.ComponentModel.DataAnnotations;

namespace Supplements.ViewModels.Profile;

public class ProfileViewModel
{
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class EditProfileViewModel
{
    [Required(ErrorMessage = "Full name is required")]
    [StringLength(100)]
    public string FullName { get; set; } = string.Empty;

    [Phone(ErrorMessage = "Invalid phone number")]
    public string? PhoneNumber { get; set; }
}

public class ChangePasswordViewModel
{
    [Required(ErrorMessage = "Current password is required")]
    [DataType(DataType.Password)]
    public string CurrentPassword { get; set; } = string.Empty;

    [Required(ErrorMessage = "New password is required")]
    [StringLength(100, MinimumLength = 8, ErrorMessage = "Password must be at least 8 characters")]
    [DataType(DataType.Password)]
    public string NewPassword { get; set; } = string.Empty;

    [Required(ErrorMessage = "Confirm new password is required")]
    [Compare("NewPassword", ErrorMessage = "Passwords do not match")]
    [DataType(DataType.Password)]
    public string ConfirmNewPassword { get; set; } = string.Empty;
}

public class AddressListViewModel
{
    public List<AddressItemViewModel> Addresses { get; set; } = new();
}

public class AddressItemViewModel
{
    public Guid Id { get; set; }
    public string Country { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string Street { get; set; } = string.Empty;
    public string? BuildingNumber { get; set; }
    public string? Floor { get; set; }
    public string? Notes { get; set; }
    public bool IsDefault { get; set; }
    public string FullAddress => $"{Street}, {City}, {Country}";
}

public class AddressFormViewModel
{
    [Required(ErrorMessage = "Country is required")]
    public string Country { get; set; } = string.Empty;

    [Required(ErrorMessage = "City is required")]
    public string City { get; set; } = string.Empty;

    [Required(ErrorMessage = "Street is required")]
    public string Street { get; set; } = string.Empty;

    public string? BuildingNumber { get; set; }
    public string? Floor { get; set; }
    public string? Notes { get; set; }
    public bool IsDefault { get; set; }
}
