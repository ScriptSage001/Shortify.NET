using Shortify.NET.Applicaion.Abstractions.Repositories;
using Shortify.NET.Applicaion.Shared.Models;
using Shortify.NET.Common.FunctionalTypes;
using Shortify.NET.Common.Messaging.Abstractions;
using System.Collections.Generic;

namespace Shortify.NET.Applicaion.Url.Queries.GetAllShortenedUrls
{
    internal sealed class GetAllShortenedUrlsQueryHandler : IQueryHandler<GetAllShortenedUrlsQuery, List<ShortenedUrlDto>>
    {
        private readonly IShortenedUrlRepository _shortenedUrlRepository;

        public GetAllShortenedUrlsQueryHandler(IShortenedUrlRepository shortenedUrlRepository)
        {
            _shortenedUrlRepository = shortenedUrlRepository;
        }

        public async Task<Result<List<ShortenedUrlDto>>> Handle(GetAllShortenedUrlsQuery query, CancellationToken cancellationToken)
        {
            Guid userId = Guid.Parse(query.UserId);

            var response = await _shortenedUrlRepository.GetAllByUserIdAsync(userId);

            if (response is null)
            {
                return Result
                        .Failure<List<ShortenedUrlDto>>(
                            Error.NotFound(
                                "ShortenedUrl.NotFound", 
                                "No Shortened Url is available for this User."));
            }

            var result = response
                            .Select(
                                url => new ShortenedUrlDto(
                                                Id: url.Id,
                                                UserId: userId,
                                                OriginalUrl: url.OriginalUrl,
                                                ShortUrl: url.ShortUrl.Value,
                                                Code: url.Code,
                                                Title: url.Title,
                                                Tags: url.Tags,
                                                CreatedOnUtc: url.CreatedOnUtc,
                                                UpdatedOnUtc: url.UpdatedOnUtc
                                            ))
                            .ToList();

            return result;
        }
    }
}
