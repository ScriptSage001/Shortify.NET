﻿namespace Shortify.NET.Application.Shared.Models
{
    public record ShortenedUrlDto(
            Guid Id,
            Guid UserId,
            string OriginalUrl,
            string ShortUrl,
            string Code,
            string? Title,
            List<string>? Tags,
            DateTime CreatedOnUtc,
            DateTime? UpdatedOnUtc,
            bool RowStatus
        );
}
