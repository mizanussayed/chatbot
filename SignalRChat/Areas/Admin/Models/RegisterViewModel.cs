using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace SignalRChat.Areas.Admin.Models;

public class RegisterViewModel
{
    [Required]
    [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 1)]
    public string FirstName { get; set; } = string.Empty;

    [Required]
    [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 1)]
    public string LastName { get; set; } = string.Empty;

    [Required]
    [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
    [Remote("IsUsernameUsed", "User", "Admin", ErrorMessage = "This username already used", AdditionalFields = "Id")]
    public string UserName { get; set; } = string.Empty;

    [RegularExpression(@"^(?:\+88|88)?(01[0-9]\d{8})$", ErrorMessage = "Please input valid phone number")]
    public string? PhoneNumber { get; set; }

    [Required]
    public string Gender { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    [Display(Name = "Email")]
    [Remote("IsEmailUsed", "User", "Admin", ErrorMessage = "This mail already used", AdditionalFields = "Id")]
    public string Email { get; set; } = string.Empty;

    [Required]
    [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
    [DataType(DataType.Password)]
    [Display(Name = "Password")]
    public string Password { get; set; } = string.Empty;

    [DataType(DataType.Password)]
    [Display(Name = "Confirm password")]
    [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
    public string ConfirmPassword { get; set; } = string.Empty;
}
