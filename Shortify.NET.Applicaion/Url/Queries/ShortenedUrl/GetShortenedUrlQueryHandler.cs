using Shortify.NET.Applicaion.Abstractions.Repositories;
using Shortify.NET.Applicaion.Shared.Models;
using Shortify.NET.Common.FunctionalTypes;
using Shortify.NET.Common.Messaging.Abstractions;
using Shortify.NET.Core.Errors;
using System;

namespace Shortify.NET.Applicaion.Url.Queries.ShortenedUrl
{
    internal sealed class GetShortenedUrlQueryHandler : 
        IQueryHandler<GetShortenedUrlByCodeQuery, ShortenedUrlDto>, 
        IQueryHandler<GetShortenedUrlByIdQuery, ShortenedUrlDto>
    {
        private readonly IShortenedUrlRepository _shortenedUrlRepository;

        public GetShortenedUrlQueryHandler(IShortenedUrlRepository shortenedUrlRepository)
        {
            _shortenedUrlRepository = shortenedUrlRepository;
        }

        public async Task<Result<ShortenedUrlDto>> Handle(GetShortenedUrlByCodeQuery query, CancellationToken cancellationToken)
        {
            var shortenedUrl = await _shortenedUrlRepository.GetByCodeAsync(query.Code);

            if(shortenedUrl == null)
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
                    UpdatedOnUtc: shortenedUrl.UpdatedOnUtc
                );
        }

        public async Task<Result<ShortenedUrlDto>> Handle(GetShortenedUrlByIdQuery query, CancellationToken cancellationToken)
        {
            var shortenedUrl = await _shortenedUrlRepository.GetByIdAsync(query.Id);

            if (shortenedUrl == null)
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
                    UpdatedOnUtc: shortenedUrl.UpdatedOnUtc
                );
        }
    }
}
