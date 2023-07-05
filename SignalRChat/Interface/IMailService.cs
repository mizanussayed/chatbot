using SignalRChat.DTOs;

namespace SignalRChat.Interface
{
    public interface IMailService
    {
        Task<bool> SendAsync(MailRequest mailRequest, CancellationToken cancellation);
    }
}
