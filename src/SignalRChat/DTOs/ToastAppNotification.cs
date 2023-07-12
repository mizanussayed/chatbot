namespace SignalRChat.DTOs
{
    public class ToastAppNotification
    {
        private string ToastMessage { get; set; } = string.Empty;
        private string ToastStatus { get; set; } = string.Empty;
        private int? Duration { get; set; } = null;
        public ToastAppNotification(string toastMessage, string toastStatus, int? duration = null)
        {
            ToastMessage = toastMessage;
            ToastStatus = toastStatus;
            Duration = duration;
        }
    }
}
