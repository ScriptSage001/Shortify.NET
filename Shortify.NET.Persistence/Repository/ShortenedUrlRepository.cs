using Microsoft.EntityFrameworkCore;
using Shortify.NET.Applicaion.Abstractions.Repositories;
using Shortify.NET.Core.Entites;

namespace Shortify.NET.Persistence.Repository
{
    public class ShortenedUrlRepository : IShortenedUrlRepository
    {
        private readonly AppDbContext _appDbContext;

        public ShortenedUrlRepository(AppDbContext context) 
            => _appDbContext = context;

        public void Add(ShortenedUrl shortenedUrl)
        {
            _appDbContext.Set<ShortenedUrl>().Add(shortenedUrl);
        }

        public void Delete(ShortenedUrl shortenedUrl)
        {
            _appDbContext.Set<ShortenedUrl>().Remove(shortenedUrl);
        }

        public void Update(ShortenedUrl shortenedUrl)
        {
            _appDbContext.Set<ShortenedUrl>().Update(shortenedUrl);
        }

        public async Task<ShortenedUrl?> GetByCodeAsync(string code, CancellationToken cancellationToken = default)
        {
            return await _appDbContext
                               .Set<ShortenedUrl>()
                               .Where(x => x.Code == code)
                               .FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<ShortenedUrl?> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            return await _appDbContext
                              .Set<ShortenedUrl>()
                              .Where(x => x.UserId == userId)
                              .FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<bool> IsCodeUniqueAsync(string code, CancellationToken cancellationToken = default)
        {
            return !await _appDbContext
                            .Set<ShortenedUrl>()
                            .AnyAsync(url => url.Code == code, cancellationToken);
        }
    }
}
