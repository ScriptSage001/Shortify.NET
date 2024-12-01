using Shortify.NET.Core.Entites;

namespace Shortify.NET.Application.Abstractions.Repositories
{
    /// <summary>
    /// Defines the interface for Role Repository.
    /// </summary>
    public interface IRoleRepository
    {
        /// <summary>
        /// Gets a role by its name.
        /// </summary>
        /// <param name="name">The role name.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The role.</returns>
        Task<Role?> GetByNameAsync(string name, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets a role by its ID.
        /// </summary>
        /// <param name="roleId">The role ID.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The role.</returns>
        Task<Role?> GetByIdAsync(int roleId, CancellationToken cancellationToken = default);
        
        /// <summary>
        /// Gets all role names by their IDs.
        /// </summary>
        /// <param name="roleIds">The list of role IDs.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The list of role names.</returns>
        Task<List<string>> GetAllRoleNamesByIdsAsync(List<int> roleIds, CancellationToken cancellationToken = default);
    }
}