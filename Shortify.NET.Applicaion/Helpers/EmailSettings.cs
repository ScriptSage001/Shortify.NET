namespace Shortify.NET.Applicaion.Helpers
{
    public class EmailSettings
    {
        public string SenderEmail { get; set; } = string.Empty;

        public string Password { get; set; } = string.Empty;

        public string Host { get; set; } = string.Empty;

        public string DisplayName { get; set; } = string.Empty;

        public int Port { get; set; }

        public int OtpLifeSpanInMinutes { get; set; }
    }
}
