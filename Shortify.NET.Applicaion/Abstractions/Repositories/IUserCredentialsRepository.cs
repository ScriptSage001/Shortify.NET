using Shortify.NET.Core.Entites;

namespace Shortify.NET.Applicaion.Abstractions.Repositories
{
    public interface IUserCredentialsRepository
    {
        Task<UserCredentials?> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);

        void Add(UserCredentials userCredentials);

        void Update(UserCredentials userCredentials);

        void Delete(UserCredentials userCredentials);
    }
}
