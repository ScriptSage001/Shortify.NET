using Shortify.NET.Core.Entites;

namespace Shortify.NET.Applicaion.Abstractions.Repositories
{
    public interface IRoleRepository
    {
        Task<Role?> GetByNameAsync(string name, CancellationToken cancellationToken = default);

        Task<List<string>> GetAllRoleNamesByIdsAsync(List<int> userRoleIds, CancellationToken cancellationToken);
    }
}