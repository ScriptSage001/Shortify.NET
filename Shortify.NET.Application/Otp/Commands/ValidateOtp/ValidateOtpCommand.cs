using Shortify.NET.Common.Messaging.Abstractions;

namespace Shortify.NET.Application.Otp.Commands.ValidateOtp
{
    public record ValidateOtpCommand(string Email, string Otp) : ICommand<string>;
}
