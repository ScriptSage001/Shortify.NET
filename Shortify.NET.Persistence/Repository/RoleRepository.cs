using Microsoft.EntityFrameworkCore;
using Shortify.NET.Applicaion.Abstractions.Repositories;
using Shortify.NET.Core.Entites;

namespace Shortify.NET.Persistence.Repository
{
    public class RoleRepository(AppDbContext appDbContext)
        : IRoleRepository
    {
        private readonly AppDbContext _appDbContext = appDbContext;
        
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

        public async Task<List<string>> GetAllRoleNamesByIdsAsync(
            List<int> userRoleIds, 
            CancellationToken cancellationToken)
        {
            return await _appDbContext
                            .Set<Role>()
                            .AsNoTracking()
                            .Where(r => userRoleIds.Contains(r.Id))
                            .Select(r => r.Name)
                            .ToListAsync(cancellationToken);
        }
    }
}