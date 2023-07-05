using Newtonsoft.Json;

namespace SignarlRChat.Interface
{
    public interface IToastNotification
    {
        Task Success(string message, int? duration = null);
        Task Error(string message, int? duration = null);
        Task Warning(string message, int? duration = null);
        Task Info(string message, int? duration = null);
        void Custom(string message, int? durationInSeconds = null, string backgroundColor = "black", string iconClassName = "home");

    }
}
