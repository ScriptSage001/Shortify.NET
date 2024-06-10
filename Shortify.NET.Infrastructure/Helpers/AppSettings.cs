namespace Shortify.NET.Infrastructure.Helpers
{
    public class AppSettings
    {
        public string Secret { get; init; } = string.Empty;
        public string Issuer { get; init; } = string.Empty;
        public int TokenExpirationTime { get; init; }
        public int RefreshTokenExpirationTimeInDays { get; init; }
        public int ValidateOtpTokenExpirationTimeInMin { get; init; }
        public string ClientSecret { get; init; } = string.Empty;
    }
}
