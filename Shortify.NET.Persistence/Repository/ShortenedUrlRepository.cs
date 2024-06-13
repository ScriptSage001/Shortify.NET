using Microsoft.EntityFrameworkCore;
using Shortify.NET.Applicaion.Abstractions.Repositories;
using Shortify.NET.Core.Entites;
using System.Linq.Expressions;

namespace Shortify.NET.Persistence.Repository
{
    public class ShortenedUrlRepository(AppDbContext context) 
        : IShortenedUrlRepository
    {
        private readonly AppDbContext _appDbContext = context;

        #region Private Methods

        private async Task<ShortenedUrl?> GetShortenedUrlAsync(
            Expression<Func<ShortenedUrl, bool>> predicate,
            bool asNoTracking = false,
            CancellationToken cancellationToken = default)
        {
            IQueryable<ShortenedUrl> query = _appDbContext.Set<ShortenedUrl>();

            if (asNoTracking)
            {
                query.AsNoTracking();
            }

            return await query.FirstOrDefaultAsync(predicate, cancellationToken);
        }

        #endregion

        #region Public Methods

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

        public async Task<ShortenedUrl?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
            => await GetShortenedUrlAsync(
                        shortenedUrl => shortenedUrl.Id == id,
                        true,
                        cancellationToken);

        public async Task<ShortenedUrl?> GetByCodeAsync(string code, CancellationToken cancellationToken = default)
            => await GetShortenedUrlAsync(
                        shortenedUrl => shortenedUrl.Code == code,
                        true,
                        cancellationToken);

        public async Task<ShortenedUrl?> GetLatestByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
            => await GetShortenedUrlAsync(
                        shortenedUrl => shortenedUrl.UserId == userId,
                        true,
                        cancellationToken);

        public async Task<List<ShortenedUrl>?> GetAllByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            return await _appDbContext
                              .Set<ShortenedUrl>()
                              .AsNoTracking()
                              .Where(x => x.UserId == userId)
                              .ToListAsync(cancellationToken);
        }

        public async Task<bool> IsCodeUniqueAsync(string code, CancellationToken cancellationToken = default)
        {
            return !await _appDbContext
                            .Set<ShortenedUrl>()
                            .AnyAsync(url => url.Code == code, cancellationToken);
        }

        #endregion
    }
}
