using Microsoft.EntityFrameworkCore;
using Shortify.NET.Applicaion.Abstractions.Repositories;
using Shortify.NET.Core.Entites;
using Shortify.NET.Core.ValueObjects;

namespace Shortify.NET.Persistence.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _appDbContext;

        public UserRepository(AppDbContext appDbContext) => _appDbContext = appDbContext;

        public async Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _appDbContext
                            .Set<User>()
                            .Where(user => user.Id == id)
                            .FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<User?> GetByUserNameAsync(UserName userName, CancellationToken cancellationToken = default)
        {
            return await _appDbContext
                            .Set<User>()
                            .Where(user => user.UserName == userName)
                            .FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<User?> GetByEmailAsync(Email email, CancellationToken cancellationToken = default)
        {
            return await _appDbContext
                            .Set<User>()
                            .Where(user => user.Email == email)
                            .FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<User?> GetByEmailAsyncWithCredentials(Email email, CancellationToken cancellationToken = default)
        {
            return await _appDbContext
                            .Set<User>()
                            .Include(user => user.UserCredentials)
                            .Where(user => user.Email == email)
                            .FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<User?> GetByUserNameAsyncWithCredentials(UserName userName, CancellationToken cancellationToken = default)
        {
            return await _appDbContext
                            .Set<User>()
                            .Include(user => user.UserCredentials)
                            .Where(user => user.UserName == userName)
                            .FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<User?> GetByIdAsyncWithCredentials(Guid id, CancellationToken cancellationToken = default)
        {
            return await _appDbContext
                            .Set<User>()
                            .Include(user => user.UserCredentials)
                            .Where(user => user.Id == id)
                            .FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<User?> GetByIdAsyncWithShortenedUrls(Guid id, CancellationToken cancellationToken = default)
        {
            return await _appDbContext
                            .Set<User>()
                            .Include(user => user.ShortenedUrls)
                            .Where(user => user.Id == id)
                            .FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<User?> GetByUserNameAsyncWithShortenedUrls(UserName userName, CancellationToken cancellationToken = default)
        {
            return await _appDbContext
                            .Set<User>()
                            .Include(user => user.ShortenedUrls)
                            .Where(user => user.UserName == userName)
                            .FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<User?> GetByEmailAsyncWithShortenedUrls(Email email, CancellationToken cancellationToken = default)
        {
            return await _appDbContext
                            .Set<User>()
                            .Include(user => user.ShortenedUrls)
                            .Where(user => user.Email == email)
                            .FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<bool> IsEmailUniqueAsync(Email email, CancellationToken cancellationToken = default)
        {
            return !await _appDbContext
                            .Set<User>()
                            .AnyAsync(user => user.Email == email, cancellationToken);
        }

        public async Task<bool> IsUserNameUniqueAsync(UserName userName, CancellationToken cancellationToken = default)
        {
            return !await _appDbContext
                            .Set<User>()
                            .AnyAsync(user => user.UserName == userName, cancellationToken);
        }

        public void Add(User user)
        {
            _appDbContext.Set<User>().Add(user);
        }

        public void Update(User user)
        {
            _appDbContext.Set<User>().Update(user);
        }

        public void Delete(User user)
        {
            _appDbContext.Set<User>().Remove(user);
        }
    }
}
