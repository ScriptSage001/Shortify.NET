using Shortify.NET.Common.Messaging.Abstractions;

namespace Shortify.NET.Applicaion.Url.Queries.ShortenedUrl
{
    public record GetShortenedUrlQuery(string Code) : IQuery<string>;
}
