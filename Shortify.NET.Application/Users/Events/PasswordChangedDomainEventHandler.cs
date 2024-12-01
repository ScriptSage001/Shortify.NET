using Shortify.NET.Common.Messaging.Abstractions;
using Shortify.NET.Core.Events;
using System.ComponentModel.DataAnnotations;
using Shortify.NET.Core.Entites;
using static Shortify.NET.Application.Shared.Constant.EmailConstants;
using System.Text;
using Shortify.NET.Application.Abstractions;
using Shortify.NET.Application.Abstractions.Repositories;
using Shortify.NET.Application.Shared;
using Shortify.NET.Application.Shared.Models;

namespace Shortify.NET.Application.Users.Events
{
    internal sealed class PasswordChangedDomainEventHandler(
        IUserRepository userRepository,
        IEmailServices emailServices) 
        : IDomainEventHandler<PasswordChangedDomainEvent>
    {
        private readonly IUserRepository _userRepository = userRepository;

        private readonly IEmailServices _emailServices = emailServices;

        public async Task Handle(PasswordChangedDomainEvent domainEvent, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByIdAsync(domainEvent.UserId, cancellationToken) ??
                       throw new ValidationException("User Not Found.");

            await SendWelcomeEmail(user, cancellationToken);
        }

        #region Private Methods

        /// <summary>
        /// To Send Welcome Email
        /// </summary>
        /// <param name="user"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        private async Task SendWelcomeEmail(User user, CancellationToken cancellationToken)
        {
            MailRequest mailRequest = new
            (
                Recipients: [user.Email.Value],
                Subject: Constant.EmailConstants.Subject.PasswordChanged,
                Body: GenerateEmailBody(user.UserName.Value)
            );

            await _emailServices.SendMailAsync(mailRequest, cancellationToken);
        }

        /// <summary>
        /// TO Prepare the Email Body
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        private static string GenerateEmailBody(string userName)
        {
            StringBuilder stringBuilder = new StringBuilder();

            stringBuilder.Append("<html lang='en'><head><style>body{font-family:Arial,sans-serif;line-height:1.6;background-color:#f4f4f4;margin:0;padding:20px;}");
            stringBuilder.Append(".container{max-width:600px;margin:auto;background:#fff;padding:20px;border-radius:5px;box-shadow:0 0 10px rgba(0,0,0,0.1);}</style>");
            stringBuilder.Append("<body><div class='container'><h2>Dear {userName},</h2>");
            stringBuilder.Append("<p>Your password for Shortify.NET is recently changed.</p>");
            stringBuilder.Append("<p>If you didn't changed the password or if you have any questions or need assistance, feel free to contact our support team.</p>");
            stringBuilder.Append("<br/><br/><p>Happy shopping!<br>Shortify.NET Team</p></div></body></html>");

            var body = stringBuilder.ToString().Replace("{userName}", userName);

            return body;
        }

        #endregion
    }
}