namespace SignarlRChat.Areas.Chat.Models
{
    public class ChatUserViewModel
    {
        public string Id { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? ProfilePhotoUrl { get; set; } = string.Empty;
        public int UnreadMessageNo { get; set; }
    }
}
