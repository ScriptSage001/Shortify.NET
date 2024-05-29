using Shortify.NET.Applicaion.Abstractions;
using Shortify.NET.Applicaion.Abstractions.Repositories;
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

        /// <summary>
        /// Constructor to initialize ShortenUrlCommandHandler
        /// </summary>
        /// <param name="urlShorteningService"></param>
        /// <param name="unitOfWork"></param>
        /// <param name="shortenedUrlRepository"></param>
        public ShortenUrlCommandHandler(IUrlShorteningService urlShorteningService, IUnitOfWork unitOfWork, IShortenedUrlRepository shortenedUrlRepository)
        {
            _urlShorteningService = urlShorteningService;
            _unitOfWork = unitOfWork;
            _shortenedUrlRepository = shortenedUrlRepository;
        }

        public async Task<Result<ShortUrl>> Handle(ShortenUrlCommand command, CancellationToken cancellationToken)
        {
            const int MaxRetries = 5;
            int retryCount = 0;

            while(retryCount < MaxRetries)
            {
                // Generate Code
                var code = await _urlShorteningService.GenerateUniqueCode(cancellationToken);

                // Generate Short Url
                string shortUrlString = $"{command.HttpRequest.Scheme}://{command.HttpRequest.Host}/{code}";
                var shortUrl = ShortUrl.Create(shortUrlString);

                // Validate Short Url
                if (shortUrl.IsFailure)
                {
                    return Result.Failure<ShortUrl>(shortUrl.Error);
                }

                // Genereate the shortened url object
                var shortenedUrl = ShortenedUrl.Create(
                    userId: null,
                    originalUrl: command.Url,
                    shortUrl: shortUrl.Value,
                    code: code
                    );

                try
                {
                    // Save in DB
                    _shortenedUrlRepository.Add(shortenedUrl);
                    await _unitOfWork.SaveChangesAsync(cancellationToken);

                    // Return
                    return shortUrl;
                }
                catch (Exception ex)
                {
                    retryCount++;

                    if (retryCount >= MaxRetries)
                    {
                        return Result.Failure<ShortUrl>(
                                    Error.Failure(
                                            "ShortUrl.FailedToCreate", 
                                            $"Failed to generate a unique short URL after multiple attempts. Exception : {ex.Message}"));
                    }
                }
            }

            return Result.Failure<ShortUrl>(
                                    Error.Failure(
                                            "ShortUrl.FailedToCreate",
                                            $"Failed to generate a unique short URL after multiple attempts."));
        }
    }
}