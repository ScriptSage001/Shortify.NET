using Microsoft.EntityFrameworkCore;
using Shortify.NET.Applicaion.Abstractions.Repositories;
using Shortify.NET.Core.Entites;
using Shortify.NET.Core.ValueObjects;
using System.Linq.Expressions;
namespace Shortify.NET.Persistence.Repository
{
    public class UserCredentialsRepository : IUserCredentialsRepository
    {
        private readonly AppDbContext _appDbContext;

        public UserCredentialsRepository(AppDbContext appDbContext)
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
        private async Task<UserCredentials?> GetUserCredentialsAsync(
            Expression<Func<UserCredentials, bool>> predicate,
            Expression<Func<UserCredentials, Object>>? includeExpression = null,
            bool asNoTracking = false,
            CancellationToken cancellationToken = default)
        {
            IQueryable<UserCredentials> query = _appDbContext.Set<UserCredentials>();

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

        /// <summary>
        /// Common Method to Reduce Repetitive Querying Logic of GetUserCredentials From User
        /// </summary>
        /// <param name="predicate"></param>
        /// <param name="asNoTracking"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        private async Task<UserCredentials?> GetUserCredentialsFromUserAsync(
            Expression<Func<User, bool>> predicate,
            bool asNoTracking = false,
            CancellationToken cancellationToken = default)
        {
            IQueryable<User> query = _appDbContext.Set<User>();
            
            if (asNoTracking)
            {
                query.AsNoTracking();
            }

            return await query
                            .Where(predicate)
                            .Select(user => user.UserCredentials)
                            .FirstOrDefaultAsync(cancellationToken);
        }

        #endregion

        #region Public Region

        public void Add(UserCredentials userCredentials)
        {
            _appDbContext.Set<UserCredentials>().Add(userCredentials);
        }

        public void Update(UserCredentials userCredentials)
        {
            _appDbContext.Set<UserCredentials>().Update(userCredentials);
        }

        public void Delete(UserCredentials userCredentials)
        {
            _appDbContext.Set<UserCredentials>().Remove(userCredentials);
        }

        public async Task<UserCredentials?> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
            => await GetUserCredentialsAsync(
                    userCreds => userCreds.UserId == userId,
                    asNoTracking: true,
                    cancellationToken: cancellationToken);

        public async Task<UserCredentials?> GetByUserNameAsync(UserName userName, CancellationToken cancellationToken = default)
            => await GetUserCredentialsFromUserAsync(
                        user => user.UserName == userName,
                        true,
                        cancellationToken);

        public async Task<UserCredentials?> GetByEmailAsync(Email email, CancellationToken cancellationToken = default)
            => await GetUserCredentialsFromUserAsync(
                        user => user.Email == email,
                        true,
                        cancellationToken);

        #endregion
    }
}
