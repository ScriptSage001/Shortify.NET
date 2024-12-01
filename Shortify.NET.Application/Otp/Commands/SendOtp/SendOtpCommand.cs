using Shortify.NET.Application.Shared;
using Shortify.NET.Common.Messaging.Abstractions;
using static Shortify.NET.Application.Shared.Constant.EmailConstants;

namespace Shortify.NET.Application.Otp.Commands.SendOtp
{
    public record SendOtpCommand(
            string Email,
            Constant.EmailConstants.OtpType OtpType
        ) : ICommand;
}
