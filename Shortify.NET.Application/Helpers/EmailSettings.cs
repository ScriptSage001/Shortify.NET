namespace Shortify.NET.Application.Helpers
{
    public class EmailSettings
    {
        public string SenderEmail { get; init; } = string.Empty;

        public string Password { get; init; } = string.Empty;

        public string Host { get; init; } = string.Empty;

        public string DisplayName { get; init; } = string.Empty;

        public int Port { get; init; }

        public int OtpLifeSpanInMinutes { get; init; }
    }
}
