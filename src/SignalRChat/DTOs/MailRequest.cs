namespace SignalRChat.DTOs
{
    public class MailRequest
    {
        // Receiver
        public List<string> To { get; }
        public List<string> BCC { get; }

        public List<string> CC { get; }

        // Sender
        public string? From { get; }

        public string? DisplayName { get; }

        public string? ReplyTo { get; }

        public string? ReplyToName { get; }

        // Content
        public string Subject { get; }

        public string? Body { get; }
        public List<IFormFile>? Attachments { get; set; } = new();

        public MailRequest(List<string> to, string subject, string? body = null, string? from = null, string? displayName = null, string? replyTo = null, string? replyToName = null, List<string>? bcc = null, List<string>? cc = null)
        {
            // Receiver
            To = to;
            BCC = bcc ?? new List<string>();
            CC = cc ?? new List<string>();

            // Sender
            From = from;
            DisplayName = displayName;
            ReplyTo = replyTo;
            ReplyToName = replyToName;

            // Content
            Subject = subject;
            Body = body;
        }

    }
}
