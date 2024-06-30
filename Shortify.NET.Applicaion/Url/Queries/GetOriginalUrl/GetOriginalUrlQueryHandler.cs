using Shortify.NET.Applicaion.Abstractions;
using Shortify.NET.Applicaion.Abstractions.Repositories;
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
            #region GetFromCache
            
            var cacheKey = $"Original_Url_{query.Code}";
            var originalUrl = await _cachingServices
                                        .GetAsync<string>(
                                            cacheKey,
                                            cancellationToken);

            #endregion

            if (originalUrl is not null)
            {
                return originalUrl;
            }

            #region GetFromDB

            var shortenedUrl = await _shortenedUrlRepository.GetByCodeAsync(query.Code, cancellationToken);

            #endregion

            if (shortenedUrl == null)
            {
                return Result.Failure<string>(DomainErrors.ShortenedUrl.ShortenedUrlNotFound);
            }

            #region SetCache

            await _cachingServices
                        .SetAsync(
                            cacheKey, 
                            shortenedUrl.OriginalUrl, 
                            cancellationToken);

            #endregion

            return shortenedUrl.OriginalUrl;
        }
    }
}
