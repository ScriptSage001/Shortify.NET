using Shortify.NET.Common.FunctionalTypes;
using Shortify.NET.Core.Entites;

namespace Shortify.NET.Applicaion.Abstractions.Repositories
{
    public interface IShortenedUrlRepository
    {
        Task<bool> IsCodeUniqueAsync(string code, CancellationToken cancellationToken = default);

        Task<ShortenedUrl?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

        Task<ShortenedUrl?> GetLatestByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);

        Task<ShortenedUrl?> GetByCodeAsync(string code, CancellationToken cancellationToken = default);

        Task<List<ShortenedUrl>?> GetAllByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);

        Task<PagedList<ShortenedUrl>?> GetByIdWithFilterAndSort(
            Guid id,
            string? searchTerm,
            string? sortColumn,
            string? sortOrder,
            int page,
            int pageSize,
            CancellationToken cancellationToken = default);

        void Add(ShortenedUrl shortenedUrl);

        void Update(ShortenedUrl shortenedUrl);

        void Delete(ShortenedUrl shortenedUrl);
    }
}
