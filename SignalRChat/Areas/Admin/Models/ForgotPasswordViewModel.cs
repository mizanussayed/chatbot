using System.ComponentModel.DataAnnotations;

namespace SignarlRChat.Areas.Admin.Models
{
    public class ForgotPasswordViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
    }
}
