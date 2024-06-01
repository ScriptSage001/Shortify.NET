namespace Shortify.NET.API.Contracts
{
    public record RefreshTokenRequest(
        string AccessToken,
        string RefreshToken
        );
}
