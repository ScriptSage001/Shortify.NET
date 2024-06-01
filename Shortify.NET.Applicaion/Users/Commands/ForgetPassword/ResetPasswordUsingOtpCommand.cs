using Shortify.NET.Common.Messaging.Abstractions;

namespace Shortify.NET.Applicaion.Users.Commands.ForgetPassword
{
    public record ResetPasswordUsingOtpCommand(
                string Email,
                string NewPassword,
                string ConfirmPassword,
                string ValidateOtpToken) 
        : ICommand;
}