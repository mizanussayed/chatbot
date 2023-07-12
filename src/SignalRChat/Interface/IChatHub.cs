namespace SignalRChat.Interface
{
    public interface IChatHub
    {
        Task SendToUserAsync(Conversation conversation);

    }
}
