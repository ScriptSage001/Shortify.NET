namespace Shortify.NET.API.Contracts
{
    public sealed record LoginUsingOtpRequest(
        string Email,
        string Otp);
}
