using Shortify.NET.Application.Abstractions;
using Shortify.NET.Application.Abstractions.Repositories;
using Shortify.NET.Application.Shared;
using Shortify.NET.Application.Shared.Models;
using Shortify.NET.Common.FunctionalTypes;
using Shortify.NET.Common.Messaging.Abstractions;
using Shortify.NET.Core.Errors;

namespace Shortify.NET.Application.Url.Queries.GetOriginalUrl
{
    internal sealed class GetOriginalUrlQueryHandler(
        IShortenedUrlRepository shortenedUrlRepository,
        ICachingServices cachingServices) 
        : IQueryHandler<GetOriginalUrlQuery, string>
    {
        private readonly IShortenedUrlRepository _shortenedUrlRepository = shortenedUrlRepository;
        private readonly ICachingServices _cachingServices = cachingServices;

        public async Task<Result<string>> Handle(GetOriginalUrlQuery query, CancellationToken cancellationToken)
        {
            var cacheKey = $"{Constant.Cache.Prefixes.OriginalUrls}{query.Code}";
            
            var url = await _cachingServices
                .GetOrAddAsync<ShortenedUrlDto>(
                    cacheKey,
                    factory: async () => 
                        MapToDto((await _shortenedUrlRepository
                            .GetByCodeAsync(query.Code, cancellationToken))!),
                    cancellationToken: cancellationToken,
                    slidingExpiration: TimeSpan.FromDays(1));
            
            return url is null ? 
                Result.Failure<string>(DomainErrors.ShortenedUrl.ShortenedUrlNotFound) : 
                !url.RowStatus ? 
                    Result.Failure<string>(DomainErrors.ShortenedUrl.ShortenedUrlIsGone) : 
                    url.OriginalUrl;
        }

        /// <summary>
        /// Maps the Entity to Dto for Caching and Response Purposes
        /// </summary>
        /// <param name="shortenedUrl"></param>
        /// <returns></returns>
        private static ShortenedUrlDto? MapToDto(Core.Entites.ShortenedUrl? shortenedUrl)
        {
            return shortenedUrl is null ? 
                null : 
                new ShortenedUrlDto(
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
