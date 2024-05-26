namespace Shortify.NET.API.Contracts
{
    public sealed record LoginUserResponse(
        Guid UserId,
        string AccessToken,
        string RefreshToken,
        DateTime RefreshTokenExpirationTimeUtc);
}
