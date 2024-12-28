using Shortify.NET.Application.Abstractions.Repositories;
using Shortify.NET.Application.Shared.Models;
using Shortify.NET.Common.FunctionalTypes;
using Shortify.NET.Common.Messaging.Abstractions;
using Entity = Shortify.NET.Core.Entites;

namespace Shortify.NET.Application.Url.Queries.GetAllShortenedUrls
{
    internal sealed class GetShortenedUrlsQueryHandler(IShortenedUrlRepository shortenedUrlRepository) 
        : IQueryHandler<GetShortenedUrlsQuery, PagedList<ShortenedUrlDto>>
    {
        private readonly IShortenedUrlRepository _shortenedUrlRepository = shortenedUrlRepository;

        public async Task<Result<PagedList<ShortenedUrlDto>>> Handle(GetShortenedUrlsQuery query, CancellationToken cancellationToken)
        {
            var userId = Guid.Parse(query.UserId);

            var response = await _shortenedUrlRepository
                .GetByIdWithFilterAndSort(
                    userId, 
                    query.SearchTerm,
                    query.SortColumn,
                    query.SortOrder,
                    query.FromDate,
                    query.ToDate,
                    query.Page,
                    query.PageSize,
                    cancellationToken);

            if (response is null || response.Items.Count == 0)
            {
                return Result
                        .Failure<PagedList<ShortenedUrlDto>>(
                            Error.NoContent(
                                "ShortenedUrl.NotContent", 
                                "No Shortened Url is available for this User."));
            }

            var itemsDto = response.Items.Select(MapToDto).ToList();

            return new PagedList<ShortenedUrlDto>(
                items: itemsDto,
                page: response.Page,
                pageSize: response.PageSize,
                totalCount: response.TotalCount);
        }

        private static ShortenedUrlDto MapToDto(Entity.ShortenedUrl url) => 
            new(
                Id: url.Id,
                UserId: url.UserId,
                OriginalUrl: url.OriginalUrl,
                ShortUrl: url.ShortUrl.Value,
                Code: url.Code,
                Title: url.Title,
                Tags: url.Tags,
                CreatedOnUtc: url.CreatedOnUtc,
                UpdatedOnUtc: url.UpdatedOnUtc,
                RowStatus: url.RowStatus);
    }
}
