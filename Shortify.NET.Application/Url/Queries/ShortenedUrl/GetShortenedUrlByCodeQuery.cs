using Shortify.NET.Application.Shared.Models;
using Shortify.NET.Common.Messaging.Abstractions;

namespace Shortify.NET.Application.Url.Queries.ShortenedUrl
{
    public record GetShortenedUrlByCodeQuery(string Code) : IQuery<ShortenedUrlDto>;
}
