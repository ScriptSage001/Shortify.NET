namespace Shortify.NET.API.Contracts
{
    public record ShortenedUrlResponse(
            Guid Id,
            Guid UserId,
            string OriginalUrl,
            string ShortUrl,
            string Code,
            string? Title,
            List<string>? Tags,
            DateTime CreatedOnUtc,
            DateTime? UpdatedOnUtc
        );
}