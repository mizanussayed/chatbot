using SignarlRChat.Areas.Chat.Models;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SignarlRChat.Entity
{
    public class ChatGroup
    {
        public int Id { get; set; }
        [StringLength(50)]
        [Required(ErrorMessage = "Group Name is required")]
        public string Name { get; set; } = string.Empty;
        [StringLength(200)]
        public string? Description { get; set; }
        [StringLength(50)]
        public string AuthorId { get; set; } = string.Empty; // Created By userId
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public DateTime? UpdatedDate { get; set; }
        public bool IsDeleted { get; set; }
        [StringLength(200)]
        public string? GroupPhotoUrl { get; set; }

        [NotMapped]
        public IFormFile? GroupPhoto { get; set; }
        [NotMapped]
        public virtual List<ChatUserViewModel> ChatUsers { get; set; } = new();
    }
}
