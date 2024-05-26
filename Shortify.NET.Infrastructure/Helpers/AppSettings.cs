namespace Shortify.NET.Infrastructure.Helpers
{
    public class AppSettings
    {
        public string Secret { get; set; } = string.Empty;
        public string Issuer { get; set; } = string.Empty;
        public int TokenExpirationTime { get; set; }
        public int RefreshTokenExpirationTimeInDays { get; set; }
        public int ResetPasswordTokenExpirationTimeInMin { get; set; }
        public string ClientSecret { get; set; } = string.Empty;
    }
}
