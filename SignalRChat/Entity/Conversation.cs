using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SignalRChat.Entity
{
    public class Conversation
    {
        public int Id { get; set; }
        [StringLength(50)]
        public string FromUserId { get; set; } = string.Empty;
        public string FromUserName { get; set; } = string.Empty;
        [StringLength(50)]
        public string? ToUserId { get; set; } = string.Empty;
        [StringLength(50)]
        public string? ToUserName { get; set; } = string.Empty;
        [StringLength(500)]
        public string? TextMessage { get; set; } = string.Empty;
        public DateTime SendDate { get; set; } = DateTime.Now;
        public DateTime? ReceiveDate { get; set; }
        public bool IsSeen { get; set; }
        public DateTime? SeenDate { get; set; }
        public int GroupId { get; set; }
        [NotMapped]
        public List<IFormFile>? Files { get; set; }

        [NotMapped]
        public string ToShortTime { get; set; } = string.Empty;
        public virtual List<ConversationFile> ConversationFiles { get; set; } = new();
    }
}
