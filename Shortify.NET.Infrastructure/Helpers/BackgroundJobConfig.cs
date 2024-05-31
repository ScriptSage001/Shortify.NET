namespace Shortify.NET.Infrastructure.Helpers
{
    public class BackgroundJobConfig
    {
        public string Name { get; set; } = string.Empty;

        public bool Enabled { get; set; } = false;

        public string Schedule { get; set; } = string.Empty;
    }
}
