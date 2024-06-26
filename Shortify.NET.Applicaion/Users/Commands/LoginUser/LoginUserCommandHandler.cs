﻿using Shortify.NET.Applicaion.Abstractions;
using Shortify.NET.Applicaion.Abstractions.Repositories;
using Shortify.NET.Applicaion.Shared.Models;
using Shortify.NET.Common.FunctionalTypes;
using Shortify.NET.Common.Messaging.Abstractions;
using Shortify.NET.Core;
using Shortify.NET.Core.Entites;
using Shortify.NET.Core.Errors;
using Shortify.NET.Core.ValueObjects;

namespace Shortify.NET.Applicaion.Users.Commands.LoginUser
{
    internal class LoginUserCommandHandler(
            IUserCredentialsRepository userCredentialsRepository,
            IUserRepository userRepository,
            IAuthServices authServices,
            IUnitOfWork unitOfWork) 
        : ICommandHandler<LoginUserCommand, AuthenticationResult>
    {
        private readonly IUserCredentialsRepository _userCredentialsRepository = userCredentialsRepository;
        private readonly IUserRepository _userRepository = userRepository;
        private readonly IAuthServices _authServices = authServices;
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public async Task<Result<AuthenticationResult>> Handle(LoginUserCommand command, CancellationToken cancellationToken)
        {
            User? user = null;

            if (!string.IsNullOrWhiteSpace(command.UserName))
            {
                var userName = UserName.Create(command.UserName);

                if (userName.IsFailure)
                {
                    return Result.Failure<AuthenticationResult>(userName.Error);
                }

                user = await _userRepository.GetByUserNameAsyncWithCredentials(userName.Value, cancellationToken);
            }
            else if (!string.IsNullOrWhiteSpace(command.Email))
            {
                var email = Email.Create(command.Email);

                if (email.IsFailure)
                {
                    return Result.Failure<AuthenticationResult>(email.Error);
                }

                user = await _userRepository.GetByEmailAsyncWithCredentials(email.Value, cancellationToken);
            }            

            if (user is null) 
            {
                return Result.Failure<AuthenticationResult>(DomainErrors.User.UserNotFound);
            }

            var userCredentials = user.UserCredentials;

            var isVerified = _authServices.VerifyPasswordHash(
                                                 command.Password,
                                                 userCredentials.PasswordHash,
                                                 userCredentials.PasswordSalt);

            if (!isVerified) return Result.Failure<AuthenticationResult>(DomainErrors.UserCredentials.WrongCredentials);
            var authenticationResult = _authServices.CreateToken(user.Id, user.UserName.Value, user.Email.Value);

            userCredentials.AddOrUpdateRefreshToken(
                authenticationResult.RefreshToken,
                authenticationResult.RefreshTokenExpirationTimeUtc);

            _userCredentialsRepository.Update(userCredentials);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return authenticationResult;

        }
    }
}
