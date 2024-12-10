namespace Shortify.NET.API.Contracts
{
    public record UpdateShortenedUrlRequest(
        Guid Id,
        string OriginalUrl,
        string? Title,
        List<string>? Tags);
}