using Shortify.NET.Applicaion.Shared.Models;
using Shortify.NET.Common.FunctionalTypes;
using Shortify.NET.Common.Messaging.Abstractions;

namespace Shortify.NET.Applicaion.Url.Queries.GetAllShortenedUrls
{
    public record GetShortenedUrlsQuery(
        string UserId,
        string? SearchTerm,
        string? SortColumn,
        string? SortOrder,
        int Page,
        int PageSize) 
        : IQuery<PagedList<ShortenedUrlDto>>;
}
