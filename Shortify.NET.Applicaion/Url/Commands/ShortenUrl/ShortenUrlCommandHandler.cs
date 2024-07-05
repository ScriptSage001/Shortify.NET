using Shortify.NET.Applicaion.Abstractions;
using Shortify.NET.Applicaion.Abstractions.Repositories;
using Shortify.NET.Applicaion.Shared;
using Shortify.NET.Common.FunctionalTypes;
using Shortify.NET.Common.Messaging.Abstractions;
using Shortify.NET.Core;
using Shortify.NET.Core.Entites;
using Shortify.NET.Core.ValueObjects;

namespace Shortify.NET.Applicaion.Url.Commands.ShortenUrl
{
    internal sealed class ShortenUrlCommandHandler : ICommandHandler<ShortenUrlCommand, ShortUrl>
    {
        private readonly IUrlShorteningService _urlShorteningService;

        private readonly IUnitOfWork _unitOfWork;

        private readonly IShortenedUrlRepository _shortenedUrlRepository;

        private readonly ICachingServices _cachingServices;

        /// <summary>
        /// Constructor to initialize ShortenUrlCommandHandler
        /// </summary>
        /// <param name="urlShorteningService"></param>
        /// <param name="unitOfWork"></param>
        /// <param name="shortenedUrlRepository"></param>
        /// <param name="cachingServices"></param>
        public ShortenUrlCommandHandler(
            IUrlShorteningService urlShorteningService, 
            IUnitOfWork unitOfWork, 
            IShortenedUrlRepository shortenedUrlRepository,
            ICachingServices cachingServices)
        {
            _urlShorteningService = urlShorteningService;
            _unitOfWork = unitOfWork;
            _shortenedUrlRepository = shortenedUrlRepository;
            _cachingServices = cachingServices;
        }

        public async Task<Result<ShortUrl>> Handle(ShortenUrlCommand command, CancellationToken cancellationToken)
        {
            const int maxRetries = 5;
            var retryCount = 0;

            var userId = Guid.Parse(command.UserId);

            while(true)
            {
                // Generate Code
                var code = await _urlShorteningService.GenerateUniqueCode(cancellationToken);

                // Generate Short Url
                var shortUrlString = $"{command.HttpRequest.Scheme}://{command.HttpRequest.Host}/{code}";
                var shortUrl = ShortUrl.Create(shortUrlString);

                // Validate Short Url
                if (shortUrl.IsFailure)
                {
                    return Result.Failure<ShortUrl>(shortUrl.Error);
                }

                // Generate the shortened url object
                var shortenedUrl = ShortenedUrl.Create(
                        userId: userId,
                        originalUrl: command.Url,
                        shortUrl: shortUrl.Value,
                        code: code,
                        title: command.Title,
                        tags: command.Tags
                    );

                try
                {
                    // Save in DB
                    _shortenedUrlRepository.Add(shortenedUrl);
                    await _unitOfWork.SaveChangesAsync(cancellationToken);

                    // Set in Cache
                    await SetCache(shortenedUrl, cancellationToken);

                    // Return
                    return shortUrl;
                }
                catch (Exception ex)
                {
                    retryCount++;

                    if (retryCount >= maxRetries)
                    {
                        return Result.Failure<ShortUrl>(
                                    Error.Failure(
                                            "ShortUrl.FailedToCreate", 
                                            $"Failed to generate a unique short URL after multiple attempts. Exception : {ex.Message}"));
                    }
                }
            }
        }

        private async Task SetCache(
            ShortenedUrl shortenedUrl, 
            CancellationToken cancellationToken)
        {
            var cacheKey = $"{Constant.Cache.Prefixes.OriginalUrls}{shortenedUrl.Code}";

            await _cachingServices
                        .SetAsync(
                            cacheKey, 
                            shortenedUrl.OriginalUrl, 
                            cancellationToken,
                            slidingExpiration: TimeSpan.FromDays(1));
        }
    }
}