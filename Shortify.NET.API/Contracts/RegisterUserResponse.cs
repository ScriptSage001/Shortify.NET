namespace Shortify.NET.API.Contracts
{
    public sealed record RegisterUserResponse(
        Guid UserId,
        string AccessToken,
        string RefreshToken,
        DateTime RefreshTokenExpirationTimeUtc);
}
