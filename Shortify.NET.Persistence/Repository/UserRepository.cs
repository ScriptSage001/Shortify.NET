using Microsoft.EntityFrameworkCore;
using Shortify.NET.Applicaion.Abstractions.Repositories;
using Shortify.NET.Core.Entites;
using Shortify.NET.Core.ValueObjects;
using System.Linq.Expressions;
namespace Shortify.NET.Persistence.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _appDbContext;

        public UserRepository(AppDbContext appDbContext) 
            => _appDbContext = appDbContext;

        #region Private Methods

        /// <summary>
        /// Common Method to Reduce Repetitive Querying Logic
        /// </summary>
        /// <param name="predicate"></param>
        /// <param name="includeExpression"></param>
        /// <param name="asNoTracking"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        private async Task<User?> GetUserAsync(
            Expression<Func<User, bool>> predicate, 
            Expression<Func<User, Object>>? includeExpression = null,
            bool asNoTracking = false,
            CancellationToken cancellationToken = default)
        {
            IQueryable<User> query = _appDbContext.Set<User>();

            if (asNoTracking)
            {
                query.AsNoTracking();
            }

            if (includeExpression is not null)
            {
                query = query.Include(includeExpression);
            }

            return await query.FirstOrDefaultAsync(predicate, cancellationToken);
        }

        #endregion

        #region Public Methods

        public async Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
            => await GetUserAsync(
                            user => user.Id == id, 
                            asNoTracking: true,
                            cancellationToken: cancellationToken);

        public async Task<User?> GetByUserNameAsync(UserName userName, CancellationToken cancellationToken = default)
            => await GetUserAsync(
                            user => user.UserName == userName, 
                            asNoTracking: true,
                            cancellationToken: cancellationToken);

        public async Task<User?> GetByEmailAsync(Email email, CancellationToken cancellationToken = default)
            => await GetUserAsync(
                            user => user.Email == email,
                            asNoTracking: true,
                            cancellationToken: cancellationToken);

        public async Task<User?> GetByEmailAsyncWithCredentials(Email email, CancellationToken cancellationToken = default)
            => await GetUserAsync(
                            user => user.Email == email, 
                            user => user.UserCredentials,
                            cancellationToken: cancellationToken);

        public async Task<User?> GetByUserNameAsyncWithCredentials(UserName userName, CancellationToken cancellationToken = default)
            => await GetUserAsync(
                            user => user.UserName == userName,
                            user => user.UserCredentials,
                            cancellationToken: cancellationToken);

        public async Task<User?> GetByIdAsyncWithCredentials(Guid id, CancellationToken cancellationToken = default)
            => await GetUserAsync(
                            user => user.Id == id,
                            user => user.UserCredentials,
                            cancellationToken: cancellationToken);

        public async Task<User?> GetByIdAsyncWithShortenedUrls(Guid id, CancellationToken cancellationToken = default)
            => await GetUserAsync(
                            user => user.Id == id,
                            user => user.ShortenedUrls,
                            cancellationToken: cancellationToken);

        public async Task<User?> GetByUserNameAsyncWithShortenedUrls(UserName userName, CancellationToken cancellationToken = default)
            => await GetUserAsync(
                            user => user.UserName == userName,
                            user => user.ShortenedUrls,
                            cancellationToken: cancellationToken);

        public async Task<User?> GetByEmailAsyncWithShortenedUrls(Email email, CancellationToken cancellationToken = default)
            => await GetUserAsync(
                            user => user.Email == email,
                            user => user.ShortenedUrls,
                            cancellationToken: cancellationToken);

        public async Task<bool> IsEmailUniqueAsync(Email email, CancellationToken cancellationToken = default)
            => !await _appDbContext
                            .Set<User>()
                            .AnyAsync(
                                user => user.Email == email, 
                                cancellationToken);

        public async Task<bool> IsUserNameUniqueAsync(UserName userName, CancellationToken cancellationToken = default)
            => !await _appDbContext
                            .Set<User>()
                            .AnyAsync(
                                user => user.UserName == userName, 
                                cancellationToken);

        public void Add(User user)
            => _appDbContext
                    .Set<User>()
                    .Add(user);

        public void Update(User user)
            => _appDbContext
                    .Set<User>()
                    .Update(user);

        public void Delete(User user)
            => _appDbContext
                    .Set<User>()
                    .Remove(user);

        #endregion
    }
}
