using Shortify.NET.Core.Entites;
using Shortify.NET.Core.ValueObjects;

namespace Shortify.NET.Application.Abstractions.Repositories
{
    public interface IUserRepository
    {
        Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

        Task<User?> GetByUserNameAsync(UserName userName, CancellationToken cancellationToken = default);

        Task<User?> GetByEmailAsync(Email email, CancellationToken cancellationToken = default);

        Task<User?> GetByIdAsyncWithCredentials(Guid id, CancellationToken cancellationToken = default);

        Task<User?> GetByUserNameAsyncWithCredentials(UserName userName, CancellationToken cancellationToken = default);

        Task<User?> GetByEmailAsyncWithCredentials(Email email, CancellationToken cancellationToken = default);

        Task<User?> GetByIdAsyncWithShortenedUrls(Guid id, CancellationToken cancellationToken = default);

        Task<User?> GetByUserNameAsyncWithShortenedUrls(UserName userName, CancellationToken cancellationToken = default);

        Task<User?> GetByEmailAsyncWithShortenedUrls(Email email, CancellationToken cancellationToken = default);

        Task<User?> GetByIdAsyncWithCredentialsAndRoles(Guid id, CancellationToken cancellationToken = default);

        Task<User?> GetByUserNameAsyncWithCredentialsAndRoles(UserName userName, CancellationToken cancellationToken = default);

        Task<User?> GetByEmailAsyncWithCredentialsAndRoles(Email email, CancellationToken cancellationToken = default);
        
        Task<bool> IsEmailUniqueAsync(Email email, CancellationToken cancellationToken = default);

        Task<bool> IsUserNameUniqueAsync(UserName userName, CancellationToken cancellationToken = default);

        void Add(User user);

        void Update(User user);

        void Delete(User user);
    }
}
