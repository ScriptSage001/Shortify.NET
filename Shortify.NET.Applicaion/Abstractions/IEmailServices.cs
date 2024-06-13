using Shortify.NET.Applicaion.Shared.Models;

namespace Shortify.NET.Applicaion.Abstractions
{
    /// <summary>
    /// Interface for Email Services
    /// </summary>
    public interface IEmailServices
    {
        /// <summary>
        /// To Send Emails Asynchronously
        /// </summary>
        /// <param name="mailRequest"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task SendMailAsync(MailRequest mailRequest, CancellationToken cancellationToken);
    }
}
