namespace Shortify.NET.Infrastructure.Helpers
{
    public class BackgroundJobConfig
    {
        public string Name { get; init; } = string.Empty;

        public bool Enabled { get; init; } = false;

        public string Schedule { get; init; } = string.Empty;
    }
}
