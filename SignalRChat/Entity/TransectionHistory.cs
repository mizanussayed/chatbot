using SignarlRChat.Enum;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SignarlRChat.Entity
{
    public class TransectionHistory
    {
        public int Id { get; set; }
        public int? FromGroupId { get; set; }
        [StringLength(100)]
        public string FromUserId { get; set; } = string.Empty;
        [StringLength(100)]
        public string? FromUserName { get; set; } = string.Empty;
        public int? ToGroupId { get; set; }
        [StringLength(100)]
        public string NotifyUserId { get; set; } = string.Empty;
        [StringLength(100)]
        public string NotifyUserName { get; set; } = string.Empty;
        public int TransectionId { get; set; }
        public string? TableName { get; set; }
        [StringLength(50)]
        public string? Title { get; set; }
        [StringLength(500)]
        public string? Text { get; set; }
        [StringLength(100)]
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public TransectionType TransectionType { get; set; }
        [StringLength(100)]
        public string? TransectionTypeName { get; set; }
        public string? Url { get; set; } = string.Empty;
        public TransectionStatus TransectionStatus { get; set; }
        [StringLength(500)]
        public string? TransectionStatusName { get; set; }
        public bool IsSeen { get; set; }
        public bool IsClosed { get; set; }
        public DateTime? SeenDate { get; set; }

        [NotMapped]
        public string WhenAgo { get; set; } = string.Empty;
    }
}
