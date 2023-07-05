using SignalRChat.DTOs;
using SignalRChat.Interface;
using MailKit.Security;
using MimeKit.Utils;
using MimeKit;
using Microsoft.CodeAnalysis.Options;
using Microsoft.Extensions.Options;
using MailKit.Net.Smtp;

namespace SignalRChat.Service
{
    public class MailService : IMailService
    {
        private readonly MailSettings _settings;
        private readonly IWebHostEnvironment _env;

        public MailService(IOptions<MailSettings> settings, IWebHostEnvironment env)
        {
            _settings = settings.Value;
            _env = env;
        }
        public async Task<bool> SendAsync(MailRequest mailRequest, CancellationToken cancellation)
        {
            try
            {
                // Initialize a new instance of the MimKit.MimeMessage class
                MimeMessage email = new();

                #region Sender / Receiver
                // Sender
                email.From.Add(new MailboxAddress(mailRequest.DisplayName ?? _settings.DisplayName, mailRequest.From ?? _settings.From));
                email.Sender = new MailboxAddress(mailRequest.DisplayName ?? _settings.DisplayName, mailRequest.From ?? _settings.From);

                // Receiver
                foreach (string toAddress in mailRequest.To)
                {
                    email.To.Add(MailboxAddress.Parse(toAddress));
                }

                // Set Reply to if specified in the Multi MailRequest
                if (!string.IsNullOrEmpty(mailRequest.ReplyTo))
                {
                    email.ReplyTo.Add(new MailboxAddress(mailRequest.ReplyToName, mailRequest.ReplyTo));
                }

                // Check if a CC was specified in the request
                if (mailRequest.CC is not null && mailRequest.CC.Count > 0)
                {
                    foreach (string ccAddress in mailRequest.CC.Where(c => !string.IsNullOrWhiteSpace(c)))
                    {
                        email.Cc.Add(MailboxAddress.Parse(ccAddress.Trim()));
                    }
                }


                // Check if a BCC was specified in the request
                if (mailRequest.BCC is not null && mailRequest.BCC.Count > 0)
                {
                    foreach (string bccAddress in mailRequest.BCC.Where(bc => !string.IsNullOrWhiteSpace(bc)))
                    {
                        email.Cc.Add(MailboxAddress.Parse(bccAddress.Trim()));
                    }
                }
                #endregion


                #region Conent

                email.Subject = mailRequest.Subject;

                BodyBuilder bodyBuilder = new();

                // For Embeded images 
                //var image = bodyBuilder.LinkedResources.Add(Path.Combine(_env.WebRootPath, @"images\Screenshot (3)_212.png"));
                //image.ContentId  = MimeUtils.GenerateMessageId();
                //bodyBuilder.HtmlBody = string.Format(mailRequest.Body, image.ContentId);

                bodyBuilder.HtmlBody = mailRequest.Body;

                //Check if we got any attachments and add the to the builder for our message
                if (mailRequest.Attachments is not null && mailRequest.Attachments.Count > 0)
                {
                    byte[] attachmentFileByteArray;

                    foreach (IFormFile attachment in mailRequest.Attachments)
                    {
                        // Check if length of the file in bytes is larger than 0
                        if (attachment.Length > 0)
                        {
                            // Create a new memory stream and attach attachment to mail body
                            using (MemoryStream memoryStream = new MemoryStream())
                            {
                                // Copy the attachment to the stream
                                attachment.CopyTo(memoryStream);
                                attachmentFileByteArray = memoryStream.ToArray();
                            }
                            // Add the attachment from the byte array
                            bodyBuilder.Attachments.Add(attachment.FileName, attachmentFileByteArray, ContentType.Parse(attachment.ContentType));
                        }
                    }
                }
                email.Body = bodyBuilder.ToMessageBody();

                #endregion


                #region Send Mail

                using SmtpClient smtp = new();
                if (_settings.UseSSL)
                {
                    await smtp.ConnectAsync(_settings.Host, _settings.Port, SecureSocketOptions.SslOnConnect, cancellation);
                }
                else
                {
                    await smtp.ConnectAsync(_settings.Host, _settings.Port, SecureSocketOptions.StartTls, cancellation);
                }
                await smtp.AuthenticateAsync(_settings.UserName, _settings.Password);
                await smtp.SendAsync(email, cancellation);
                await smtp.DisconnectAsync(true, cancellation);
                #endregion
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
