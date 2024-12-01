using Shortify.NET.Application.Abstractions;
using Shortify.NET.Application.Abstractions.Repositories;
using Shortify.NET.Core.Entites;
using static Shortify.NET.Application.Shared.Constant;

namespace Shortify.NET.Persistence.Repository
{
    /// <summary>
    /// Implementation of <see cref="IRoleRepository"/> to handle role data with caching.
    /// </summary>
    /// <param name="decorated">RoleRepository</param>
    /// <param name="cachingServices"></param>
    public class CachedRoleRepository(
        RoleRepository decorated,
        ICachingServices cachingServices) 
        : IRoleRepository
    {
        private readonly RoleRepository _decorated = decorated;
        private readonly ICachingServices _cachingServices = cachingServices;

        /// <inheritdoc/>
        public async Task<Role?> GetByNameAsync(
            string name,
            CancellationToken cancellationToken = default)
        {
            var cacheKey = $"{Cache.Prefixes.RoleByName}{name}";

            var role = await _cachingServices
                .GetOrAddAsync(
                    key: cacheKey,
                    factory: () => _decorated.GetByNameAsync(name, cancellationToken),
                    slidingExpiration: TimeSpan.FromDays(30),
                    cancellationToken: cancellationToken);

            return role;
        }

        /// <inheritdoc/>
        public async Task<Role?> GetByIdAsync(
            int roleId, 
            CancellationToken cancellationToken = default)
        {
            var cacheKey = $"{Cache.Prefixes.RoleById}{roleId}";
            
            var role = await _cachingServices
                .GetOrAddAsync(
                    key: cacheKey,
                    factory: () => _decorated.GetByIdAsync(roleId, cancellationToken),
                    slidingExpiration: TimeSpan.FromDays(30),
                    cancellationToken: cancellationToken);
            
            return role;
        }
        
        /// <inheritdoc/>
        public async Task<List<string>> GetAllRoleNamesByIdsAsync(
            List<int> roleIds, 
            CancellationToken cancellationToken)
        {
            var tasks = roleIds.Select(id => GetByIdAsync(id, cancellationToken));
            var roles = await Task.WhenAll(tasks);

            return roles
                    .Where(role => role is not null)
                    .Select(role => role!.Name)
                    .ToList();
        }
    }
}