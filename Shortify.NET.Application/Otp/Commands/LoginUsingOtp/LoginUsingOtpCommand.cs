using Shortify.NET.Application.Shared.Models;
using Shortify.NET.Common.Messaging.Abstractions;

namespace Shortify.NET.Application.Otp.Commands.LoginUsingOtp
{
    public record LoginUsingOtpCommand(
                        string Email, 
                        string Otp) 
        : ICommand<AuthenticationResult>;
}
