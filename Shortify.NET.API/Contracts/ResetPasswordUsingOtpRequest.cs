namespace Shortify.NET.API.Contracts
{
    public record ResetPasswordUsingOtpRequest(
        string Email,
        string NewPassword,
        string ConfirmPassword,
        string ValidateOtpToken
        );
}
