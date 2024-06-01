namespace Shortify.NET.API.Contracts
{
    public sealed record AuthenticationResponse(
                                Guid UserId,
                                string AccessToken,
                                string RefreshToken,
                                DateTime RefreshTokenExpirationTimeUtc);
}
