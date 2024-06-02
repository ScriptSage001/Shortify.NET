using Shortify.NET.Applicaion.Shared.Models;
using Shortify.NET.Common.Messaging.Abstractions;

namespace Shortify.NET.Applicaion.Url.Queries.ShortenedUrl
{
    public record GetShortenedUrlByCodeQuery(string Code) : IQuery<ShortenedUrlDto>;
}
