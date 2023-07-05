using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SignarlRChat.Entity;
namespace SignarlRChat.Entity
{
    public class UserGroup
    {
        public int Id { get; set; }
        public int GroupId { get; set; }
        [StringLength(50)]
        public string UserId { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; }
        [StringLength(50)]
        public string CreatedBy { get; set; } = string.Empty;

        [NotMapped]
        public virtual ChatGroup ChatGroup { get; set; } = new();
        [NotMapped]
        public virtual ApplicationUser ApplicationUser { get; set; } = new();
    }
}
