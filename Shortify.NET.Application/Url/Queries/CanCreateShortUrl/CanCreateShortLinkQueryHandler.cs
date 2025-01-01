using Shortify.NET.Application.Abstractions.Repositories;
using Shortify.NET.Common.FunctionalTypes;
using Shortify.NET.Common.Messaging.Abstractions;

namespace Shortify.NET.Application.Url.Queries.CanCreateShortUrl
{
    internal class CanCreateShortLinkQueryHandler(
        IShortenedUrlRepository shortenedUrlRepository
        ) : IQueryHandler<CanCreateShortLinkQuery, bool>
    {
        private readonly IShortenedUrlRepository _shortenedUrlRepository = shortenedUrlRepository;
        
        public async Task<Result<bool>> Handle(CanCreateShortLinkQuery query, CancellationToken cancellationToken)
        {
            var userId = Guid.Parse(query.UserId);
            
            var currentMonthYear = DateTime.SpecifyKind(
                                                        new DateTime(
                                                                DateTime.UtcNow.Year, 
                                                                DateTime.UtcNow.Month, 
                                                                1),
                                                        DateTimeKind.Utc);
            var nextMonthYear = currentMonthYear.AddMonths(1);
            
            var shortenedUrlCount = (await _shortenedUrlRepository
                                                                .GetAllByUserIdAsync(
                                                                    userId,
                                                                    currentMonthYear,
                                                                    nextMonthYear,
                                                                    cancellationToken))?
                                                                .Count;

            return shortenedUrlCount is null or < 10;
        }
    }
}

