﻿using Shortify.NET.Application.Abstractions;
using Shortify.NET.Application.Abstractions.Repositories;
using Shortify.NET.Application.Shared.Models;
using Shortify.NET.Common.FunctionalTypes;
using Shortify.NET.Common.Messaging.Abstractions;
using Shortify.NET.Core;
using Shortify.NET.Core.Errors;
using Shortify.NET.Core.ValueObjects;

namespace Shortify.NET.Application.Token.Commands.GetTokenByClientSecret
{
    internal sealed class GenerateTokenByClientSecretCommandHandler(
        IUserRepository userRepository,
        IUserCredentialsRepository userCredentialsRepository,
        IRoleRepository roleRepository,
        IAuthServices authServices,
        IUnitOfWork unitOfWork) 
        : ICommandHandler<GenerateTokenByClientSecretCommand, AuthenticationResult>
    {
        private readonly IUserRepository _userRepository = userRepository;

        private readonly IUserCredentialsRepository _userCredentialsRepository = userCredentialsRepository;
        
        private readonly IRoleRepository _roleRepository = roleRepository;

        private readonly IAuthServices _authServices = authServices;

        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public async Task<Result<AuthenticationResult>> Handle(GenerateTokenByClientSecretCommand command, CancellationToken cancellationToken)
        {
            var userName = UserName.Create(command.UserName);

            if (userName.IsFailure)
            {
                return Result.Failure<AuthenticationResult>(userName.Error);
            }

            var user = await _userRepository.GetByUserNameAsyncWithCredentials(userName.Value, cancellationToken);

            if (user is null)
            {
                return Result.Failure<AuthenticationResult>(DomainErrors.User.UserNotFound);
            }

            var isValid = _authServices.ValidateClientSecret(command.ClientSecret);

            if (!isValid)
            {
                return Result.Failure<AuthenticationResult>(DomainErrors.UserCredentials.WrongCredentials);
            }

            var userRoleIds = user.UserRoles.Select(ur => ur.RoleId).ToList();
            var userRoles = await _roleRepository.GetAllRoleNamesByIdsAsync(userRoleIds, cancellationToken);
            
            var authenticationResult = _authServices
                .CreateToken(user.Id, user.UserName.Value, user.Email.Value, userRoles);

            user.UserCredentials.AddOrUpdateRefreshToken(
                                authenticationResult.RefreshToken,
                                authenticationResult.RefreshTokenExpirationTimeUtc);

            _userCredentialsRepository.Update(user.UserCredentials);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return authenticationResult;
        }
    }
}
