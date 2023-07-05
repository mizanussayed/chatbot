using SignarlRChat.DTOs;

namespace SignarlRChat.Interface
{
    public interface IMailService
    {
        Task<bool> SendAsync(MailRequest mailRequest, CancellationToken cancellation);
    }
}
