namespace SignarlRChat.Interface
{
    public interface IChatHub
    {
        Task SendToUserAsync(Conversation conversation);

    }
}
