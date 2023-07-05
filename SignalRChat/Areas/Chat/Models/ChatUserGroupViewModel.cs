namespace SignarlRChat.Areas.Chat.Models
{
    public class ChatUserGroupViewModel
    {
        public string LoginUserId { get; set; } = string.Empty;
        public string LoginUserFullName { get; set; } = string.Empty;
        public List<ChatUserViewModel> ChatUsers = new();
        public List<ChatGroup> ChatGroupsList = new();
        public List<TransectionHistory> TransectionHistries { get; set; } = new();
    }
}
