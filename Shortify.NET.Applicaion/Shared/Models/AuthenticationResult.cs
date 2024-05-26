namespace Shortify.NET.Applicaion.Shared.Models
{
    public sealed record AuthenticationResult(
                                Guid UserId,
                                string AccessToken,
                                string RefreshToken,
                                DateTime RefreshTokenExpirationTimeUtc);
}
