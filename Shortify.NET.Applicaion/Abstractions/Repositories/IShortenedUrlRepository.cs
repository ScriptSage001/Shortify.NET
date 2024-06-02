using Shortify.NET.Core.Entites;

namespace Shortify.NET.Applicaion.Abstractions.Repositories
{
    public interface IShortenedUrlRepository
    {
        Task<bool> IsCodeUniqueAsync(string code, CancellationToken cancellationToken = default);

        Task<ShortenedUrl?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

        Task<ShortenedUrl?> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);

        Task<ShortenedUrl?> GetByCodeAsync(string code, CancellationToken cancellationToken = default);

        Task<List<ShortenedUrl>?> GetAllByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);

        void Add(ShortenedUrl shortenedUrl);

        void Update(ShortenedUrl shortenedUrl);

        void Delete(ShortenedUrl shortenedUrl);
    }
}
