using Microsoft.EntityFrameworkCore;
using Shortify.NET.Application.Abstractions.Repositories;
using Shortify.NET.Common.FunctionalTypes;
using Shortify.NET.Core.Entites;
using System.Linq.Expressions;
using static Shortify.NET.Persistence.Constants.RepositoryConstants.SortConstants;

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

        public async Task<List<ShortenedUrl>?> GetAllByUserIdAsync(
            Guid userId, 
            DateTime? fromDate,
            DateTime? toDate, 
            CancellationToken cancellationToken = default)
        {
            var query = _appDbContext
                                            .Set<ShortenedUrl>()
                                            .AsNoTracking()
                                            .Where(x =>
                                                x.UserId == userId &&
                                                x.RowStatus);
            
            if (fromDate is not null && toDate is not null)
            {
                query = query.Where(url => url.CreatedOnUtc >= fromDate && url.CreatedOnUtc < toDate);
            }
            
            return await query.ToListAsync(cancellationToken);
        }

        public async Task<bool> IsCodeUniqueAsync(string code, CancellationToken cancellationToken = default)
        {
            return !await _appDbContext
                            .Set<ShortenedUrl>()
                            .AnyAsync(url => url.Code == code, cancellationToken);
        }

        public async Task<PagedList<ShortenedUrl>?> GetByIdWithFilterAndSort(
            Guid userId, 
            string? searchTerm, 
            string? sortColumn, 
            string? sortOrder, 
            DateTime? fromDate,
            DateTime? toDate,
            int page, 
            int pageSize, 
            CancellationToken cancellationToken = default)
        {
            IQueryable<ShortenedUrl> query = _appDbContext
                                                .Set<ShortenedUrl>()
                                                .AsNoTracking()
                                                .Where(x => 
                                                    x.UserId == userId &&
                                                    x.RowStatus);

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                query = query.Where(url =>
                                       (url.Title != null && url.Title.Contains(searchTerm)) ||
                                       (url.Tags != null && url.Tags.Contains(searchTerm)));
            }

            if (fromDate is not null && toDate is not null)
            {
                query = query.Where(url => url.CreatedOnUtc >= fromDate && url.CreatedOnUtc <= toDate);
            }

            var sortExpression = GetSortProperty(sortColumn);
            query = sortOrder?.ToLower() == SortOrder.Descending
                ? query.OrderByDescending(sortExpression)
                : query.OrderBy(sortExpression);

            var totalCount = await query.CountAsync(cancellationToken);
            var items = await query
                                .Skip((page - 1) * pageSize)
                                .Take(pageSize)
                                .ToListAsync(cancellationToken);

            var urls = new PagedList<ShortenedUrl>(
                items: items, 
                page: page, 
                pageSize: pageSize, 
                totalCount: totalCount);

            return urls;
        }

        #endregion

        #region Private Methods

        private static Expression<Func<ShortenedUrl, object>> GetSortProperty(string? sortColumn) => 
            sortColumn?.ToLower() switch
            {
                SortProperty.Title => url => url.Title ?? string.Empty,
                SortProperty.CreatedOn => url => url.CreatedOnUtc,
                SortProperty.UpdatedOn => url => url.UpdatedOnUtc ?? DateTime.MinValue,
                _ => url => url.Id
            };

        #endregion
    }
}
