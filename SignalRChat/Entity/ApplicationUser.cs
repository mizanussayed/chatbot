using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace SignarlRChat.Entity
{
    public class ApplicationUser : IdentityUser
    {
        [StringLength(100)]
        public string FirstName { get; set; } = string.Empty;
        [StringLength(100)]
        public string LastName { get; set; } = string.Empty;
        [StringLength(200)]
        public string? ProfilePhotoUrl { get; set; }
        [StringLength(20)]
        public string Gender { get; set; } = string.Empty;

    }
}
