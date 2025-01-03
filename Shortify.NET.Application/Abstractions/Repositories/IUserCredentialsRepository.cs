﻿using Shortify.NET.Core.Entites;
using Shortify.NET.Core.ValueObjects;

namespace Shortify.NET.Application.Abstractions.Repositories
{
    public interface IUserCredentialsRepository
    {
        Task<UserCredentials?> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);

        Task<UserCredentials?> GetByUserNameAsync(UserName userName, CancellationToken cancellationToken = default);

        Task<UserCredentials?> GetByEmailAsync(Email email, CancellationToken cancellationToken = default);

        void Add(UserCredentials userCredentials);

        void Update(UserCredentials userCredentials);

        void Delete(UserCredentials userCredentials);
    }
}
