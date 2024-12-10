using Shortify.NET.Application.Shared.Models;
using Shortify.NET.Common.FunctionalTypes;
using Shortify.NET.Common.Messaging.Abstractions;

namespace Shortify.NET.Application.Url.Queries.GetAllShortenedUrls
{
    public record GetShortenedUrlsQuery(
        string UserId,
        string? SearchTerm,
        string? SortColumn,
        string? SortOrder,
        DateTime? FromDate,
        DateTime? ToDate,
        int Page,
        int PageSize) 
        : IQuery<PagedList<ShortenedUrlDto>>;
}
