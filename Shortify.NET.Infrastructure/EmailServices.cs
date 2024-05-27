using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using Shortify.NET.Applicaion.Abstractions;
using Shortify.NET.Applicaion.Shared.Models;
using Shortify.NET.Infrastructure.Helpers;

namespace Shortify.NET.Infrastructure
{
    public class EmailServices : IEmailServices
    {
        private readonly EmailSettings _emailSettings;

        public EmailServices(IOptions<EmailSettings> options)
        {
            _emailSettings = options.Value;
        }

        public async Task SendMailAsync(MailRequest mailRequest, CancellationToken cancellationToken)
        {
            var email = PrepareMail(mailRequest);

            using var smtp = new SmtpClient();
            smtp.Connect(
                    _emailSettings.Host,
                    _emailSettings.Port,
                    SecureSocketOptions.StartTls,
                    cancellationToken);

            smtp.Authenticate(
                    _emailSettings.SenderEmail,
                    _emailSettings.Password,
                    cancellationToken);

            await smtp.SendAsync(email, cancellationToken);

            smtp.Disconnect(true, cancellationToken);
        }

        private MimeMessage PrepareMail(MailRequest mailRequest)
        {
            var email = new MimeMessage()
            {
                Sender = MailboxAddress.Parse(_emailSettings.SenderEmail),
                Subject = mailRequest.Subject,
                Body = new BodyBuilder()
                { 
                    HtmlBody = mailRequest.Body 
                }
                .ToMessageBody()
            };

            foreach (string recipient in mailRequest.Recipients)
            {
                email.To.Add(MailboxAddress.Parse(recipient));
            }

            return email;
        }
    }
}
