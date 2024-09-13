namespace Shortify.NET.API.Contracts
{
    public record UpdateShortenedUrlRequest(
        Guid Id,
        string? Title,
        List<string>? Tags);
}