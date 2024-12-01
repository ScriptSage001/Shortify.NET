using Shortify.NET.Application.Abstractions.Repositories;
using Shortify.NET.Application.Shared.Models;
using Shortify.NET.Common.FunctionalTypes;
using Shortify.NET.Common.Messaging.Abstractions;
using Shortify.NET.Core.Errors;

namespace Shortify.NET.Application.Url.Queries.ShortenedUrl
{
    internal sealed class GetShortenedUrlQueryHandler(IShortenedUrlRepository shortenedUrlRepository) : 
        IQueryHandler<GetShortenedUrlByCodeQuery, ShortenedUrlDto>, 
        IQueryHandler<GetShortenedUrlByIdQuery, ShortenedUrlDto>
    {
        private readonly IShortenedUrlRepository _shortenedUrlRepository = shortenedUrlRepository;

        public async Task<Result<ShortenedUrlDto>> Handle(GetShortenedUrlByCodeQuery query, CancellationToken cancellationToken)
        {
            var shortenedUrl = await _shortenedUrlRepository.GetByCodeAsync(query.Code, cancellationToken);

            if(shortenedUrl == null || !shortenedUrl.RowStatus)
            {
                return Result.Failure<ShortenedUrlDto>(DomainErrors.ShortenedUrl.ShortenedUrlNotFound);
            }
            
            return new ShortenedUrlDto(
                    Id: shortenedUrl.Id,
                    UserId: shortenedUrl.UserId,
                    OriginalUrl: shortenedUrl.OriginalUrl,
                    ShortUrl: shortenedUrl.ShortUrl.Value,
                    Code: shortenedUrl.Code,
                    Title: shortenedUrl.Title,
                    Tags: shortenedUrl.Tags,
                    CreatedOnUtc: shortenedUrl.CreatedOnUtc,
                    UpdatedOnUtc: shortenedUrl.UpdatedOnUtc,
                    RowStatus: shortenedUrl.RowStatus
                );
        }

        public async Task<Result<ShortenedUrlDto>> Handle(GetShortenedUrlByIdQuery query, CancellationToken cancellationToken)
        {
            var shortenedUrl = await _shortenedUrlRepository.GetByIdAsync(query.Id, cancellationToken);

            if (shortenedUrl == null || !shortenedUrl.RowStatus)
            {
                return Result.Failure<ShortenedUrlDto>(DomainErrors.ShortenedUrl.ShortenedUrlNotFound);
            }

            return new ShortenedUrlDto(
                    Id: shortenedUrl.Id,
                    UserId: shortenedUrl.UserId,
                    OriginalUrl: shortenedUrl.OriginalUrl,
                    ShortUrl: shortenedUrl.ShortUrl.Value,
                    Code: shortenedUrl.Code,
                    Title: shortenedUrl.Title,
                    Tags: shortenedUrl.Tags,
                    CreatedOnUtc: shortenedUrl.CreatedOnUtc,
                    UpdatedOnUtc: shortenedUrl.UpdatedOnUtc,
                    RowStatus: shortenedUrl.RowStatus
                );
        }
    }
}
