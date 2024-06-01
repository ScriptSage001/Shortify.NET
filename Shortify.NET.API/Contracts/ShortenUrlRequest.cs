namespace Shortify.NET.API.Contracts
{
    public record ShortenUrlRequest(
            string Url,
            string? Title,
            List<string>? Tags
        );
}
