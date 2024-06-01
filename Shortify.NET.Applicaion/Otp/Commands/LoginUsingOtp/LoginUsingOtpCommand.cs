using Shortify.NET.Applicaion.Shared.Models;
using Shortify.NET.Common.Messaging.Abstractions;

namespace Shortify.NET.Applicaion.Otp.Commands.LoginUsingOtp
{
    public record LoginUsingOtpCommand(
                        string Email, 
                        string Otp) 
        : ICommand<AuthenticationResult>;
}
