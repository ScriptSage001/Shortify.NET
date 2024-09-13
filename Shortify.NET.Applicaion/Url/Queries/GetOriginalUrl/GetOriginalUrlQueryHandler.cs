using Shortify.NET.Applicaion.Abstractions;
using Shortify.NET.Applicaion.Abstractions.Repositories;
using Shortify.NET.Applicaion.Shared;
using Shortify.NET.Common.FunctionalTypes;
using Shortify.NET.Common.Messaging.Abstractions;
using Shortify.NET.Core.Errors;

namespace Shortify.NET.Applicaion.Url.Queries.GetOriginalUrl
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
            
            var originalUrl = await _cachingServices
                .GetOrAddAsync<string>(
                    cacheKey,
                    factory: async () => 
                        (await _shortenedUrlRepository
                            .GetByCodeAsync(query.Code, cancellationToken))?
                        .OriginalUrl,
                    cancellationToken: cancellationToken,
                    slidingExpiration: TimeSpan.FromDays(1));
            
            return originalUrl ?? 
                   Result.Failure<string>(DomainErrors.ShortenedUrl.ShortenedUrlNotFound);
        }
    }
}
