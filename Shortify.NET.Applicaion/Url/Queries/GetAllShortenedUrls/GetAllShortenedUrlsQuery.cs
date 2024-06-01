using Shortify.NET.Applicaion.Shared.Models;
using Shortify.NET.Common.Messaging.Abstractions;

namespace Shortify.NET.Applicaion.Url.Queries.GetAllShortenedUrls
{
    public record GetAllShortenedUrlsQuery(string UserId) : IQuery<List<ShortenedUrlDto>>;
}
