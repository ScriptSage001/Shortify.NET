using Microsoft.EntityFrameworkCore;
using Shortify.NET.Applicaion.Abstractions.Repositories;
using Shortify.NET.Core.Entites;

namespace Shortify.NET.Persistence.Repository
{
    /// <summary>
    /// Implementation of <see cref="IRoleRepository"/> to get Role data from database.
    /// </summary>
    /// <param name="appDbContext"></param>
    public class RoleRepository(AppDbContext appDbContext)
        : IRoleRepository
    {
        private readonly AppDbContext _appDbContext = appDbContext;
        
        /// <inheritdoc/>
        public async Task<Role?> GetByNameAsync(
            string name,
            CancellationToken cancellationToken = default)
        {
            return await _appDbContext
                            .Set<Role>()
                            .AsNoTracking()
                            .FirstOrDefaultAsync(
                                role => role.Name.Equals(name), 
                                cancellationToken);
        }

        /// <inheritdoc/>
        public async Task<Role?> GetByIdAsync(
            int roleId, 
            CancellationToken cancellationToken = default)
        {
            return await _appDbContext
                            .Set<Role>()
                            .AsNoTracking()
                            .Where(r => roleId == r.Id)
                            .FirstOrDefaultAsync(cancellationToken);
        }
        
        /// <inheritdoc/>
        public async Task<List<string>> GetAllRoleNamesByIdsAsync(
            List<int> roleIds, 
            CancellationToken cancellationToken = default)
        {
            return await _appDbContext
                            .Set<Role>()
                            .AsNoTracking()
                            .Where(r => roleIds.Contains(r.Id))
                            .Select(r => r.Name)
                            .ToListAsync(cancellationToken);
        }
    }
}