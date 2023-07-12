using System.ComponentModel.DataAnnotations;

namespace SignalRChat.Areas.Admin.Models;

public class ForgotPasswordViewModel
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;
}
