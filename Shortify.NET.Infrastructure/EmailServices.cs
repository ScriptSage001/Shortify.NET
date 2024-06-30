using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using Shortify.NET.Applicaion.Abstractions;
using Shortify.NET.Applicaion.Shared.Models;
using Shortify.NET.Infrastructure.Helpers;

namespace Shortify.NET.Infrastructure
{
    public class EmailServices(IOptions<EmailSettings> options) : IEmailServices
    {
        private readonly EmailSettings _emailSettings = options.Value;

        public async Task SendMailAsync(MailRequest mailRequest, CancellationToken cancellationToken)
        {
            var email = PrepareMail(mailRequest);

            using var smtp = new SmtpClient();
            await smtp.ConnectAsync(_emailSettings.Host,
                                    _emailSettings.Port,
                                    SecureSocketOptions.StartTls,
                                    cancellationToken);

            await smtp.AuthenticateAsync(_emailSettings.SenderEmail,
                                         _emailSettings.Password,
                                         cancellationToken);

            await smtp.SendAsync(email, cancellationToken);

            await smtp.DisconnectAsync(true, cancellationToken);
        }

        private MimeMessage PrepareMail(MailRequest mailRequest)
        {
            var email = new MimeMessage
            {
                Sender = MailboxAddress.Parse(_emailSettings.SenderEmail),
                Subject = mailRequest.Subject,
                Body = new BodyBuilder
                { 
                    HtmlBody = mailRequest.Body 
                }
                .ToMessageBody()
            };

            foreach (var recipient in mailRequest.Recipients)
            {
                email.To.Add(MailboxAddress.Parse(recipient));
            }

            return email;
        }
    }
}
