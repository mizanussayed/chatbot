using System.ComponentModel.DataAnnotations;

namespace SignalRChat.Entity;

public class ConversationFile
{
    public int Id { get; set; }
    [StringLength(200)]
    public string FileName { get; set; } = string.Empty;
    [StringLength(50)]
    public string FileType { get; set; } = string.Empty;
    //[StringLength(20)]
    //public string Extension { get; set; } = string.Empty;
    [StringLength(10)]
    public string FileSize { get; set; } = string.Empty;
    [StringLength(200)]
    public string FileUrl { get; set; } = string.Empty;
    public int ConversationId { get; set; }
    //public DateTime SendDate { get; set; }
    //public string FromUserId { get; set; } = string.Empty;
    //public string? ToUserId { get; set; } = string.Empty;
    //public int? GroupId { get; set; }
}
