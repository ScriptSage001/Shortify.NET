using Shortify.NET.Common.Messaging.Abstractions;

namespace Shortify.NET.Applicaion.Otp.Commands.ValidateOtp
{
    public record ValidateOtpCommand(string Email, string Otp) : ICommand<string>;
}
