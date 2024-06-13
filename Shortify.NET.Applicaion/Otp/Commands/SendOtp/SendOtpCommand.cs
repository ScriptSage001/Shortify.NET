using Shortify.NET.Common.Messaging.Abstractions;
using static Shortify.NET.Applicaion.Shared.Constant.EmailConstants;

namespace Shortify.NET.Applicaion.Otp.Commands.SendOtp
{
    public record SendOtpCommand(
            string Email,
            OtpType OtpType
        ) : ICommand;
}
