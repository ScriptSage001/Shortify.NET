namespace Shortify.NET.Infrastructure.Helpers
{
    public class BackgroundJobConfig
    {
        public string Name { get; } = string.Empty;

        public bool Enabled { get; } = false;

        public string Schedule { get; } = string.Empty;
    }
}
