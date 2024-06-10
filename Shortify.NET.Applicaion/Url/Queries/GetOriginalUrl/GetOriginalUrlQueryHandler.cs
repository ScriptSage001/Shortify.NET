using Shortify.NET.Applicaion.Abstractions.Repositories;
using Shortify.NET.Common.FunctionalTypes;
using Shortify.NET.Common.Messaging.Abstractions;
using Shortify.NET.Core.Errors;

namespace Shortify.NET.Applicaion.Url.Queries.GetOriginalUrl
{
    internal sealed class GetOriginalUrlQueryHandler(IShortenedUrlRepository shortenedUrlRepository) 
        : IQueryHandler<GetOriginalUrlQuery, string>
    {
        private readonly IShortenedUrlRepository _shortenedUrlRepository = shortenedUrlRepository;

        public async Task<Result<string>> Handle(GetOriginalUrlQuery query, CancellationToken cancellationToken)
        {
            var shortenedUrl = await _shortenedUrlRepository.GetByCodeAsync(query.Code, cancellationToken);

            if (shortenedUrl == null)
            {
                return Result.Failure<string>(DomainErrors.ShortenedUrl.ShortenedUrlNotFound);
            }

            return shortenedUrl.OriginalUrl;
        }
    }
}
