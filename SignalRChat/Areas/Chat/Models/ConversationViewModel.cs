namespace SignarlRChat.Areas.Chat.Models
{
    public class ConversationViewModel
    {
        public string LoginUserId { get; set; } = string.Empty;
        public string FromUserName { get; set; } = string.Empty;
        public string? LoginUserProfilePhotoUrl { get; set; }
        public string ToUserName { get; set; } = string.Empty;
        public string ToUserId { get; set; } = string.Empty;
        public string? ToUserProfilePhotoUrl { get; set; } 
        public int GroupId { get; set; }
        public string GroupName { get; set; } = string.Empty;
        public string EmptyMessage { get; set; } = string.Empty;
        public string GroupAuthorId { get; set; } = string.Empty;
        public string? GroupPhotoUrl { get; set; }

        public List<Conversation> Conversations { get; set; } = new();
    }
}
