namespace SignarlRChat.Areas.Chat.Models
{
    public class GroupUsersViewModel
    {
        public string LoginUserId { get; set; } = string.Empty;
        public string AuthorId { get; set; } = string.Empty;
        public string GroupName { get; set; } = string.Empty;
        public string? GroupPhotoUrl { get; set; }
        public List<UserGroup> UserGroups = new();
    }
}
