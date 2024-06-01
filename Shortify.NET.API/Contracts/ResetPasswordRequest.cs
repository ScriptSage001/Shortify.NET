namespace Shortify.NET.API.Contracts
{
    public record ResetPasswordRequest(
        string OldPassword,
        string NewPassword,
        string ConfirmPassword
        );
}
