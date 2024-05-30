﻿using Microsoft.EntityFrameworkCore;
using Shortify.NET.Applicaion.Abstractions.Repositories;
using Shortify.NET.Core.Entites;

namespace Shortify.NET.Persistence.Repository
{
    public class UserCredentialsRepository : IUserCredentialsRepository
    {
        private readonly AppDbContext _appDbContext;

        public UserCredentialsRepository(AppDbContext appDbContext)
            => _appDbContext = appDbContext;

        public async Task<UserCredentials?> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            return await _appDbContext
                            .Set<UserCredentials>()
                            .Where(creds => creds.UserId == userId)
                            .FirstOrDefaultAsync(cancellationToken);
        }

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
    }
}