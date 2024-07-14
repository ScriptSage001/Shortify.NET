using Shortify.NET.Applicaion.Abstractions;
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
            IRoleRepository roleRepository,
            IAuthServices authServices,
            IUnitOfWork unitOfWork) 
        : ICommandHandler<LoginUserCommand, AuthenticationResult>
    {
        private readonly IUserCredentialsRepository _userCredentialsRepository = userCredentialsRepository;
        private readonly IUserRepository _userRepository = userRepository;
        private readonly IRoleRepository _roleRepository = roleRepository;
        private readonly IAuthServices _authServices = authServices;
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public async Task<Result<AuthenticationResult>> Handle(LoginUserCommand command, CancellationToken cancellationToken)
        {
            var (user, error) = await GetUserAsync(command, cancellationToken);

            if (user is null) 
            {
                return Result.Failure<AuthenticationResult>(DomainErrors.User.UserNotFound);
            }
            
            if (error is not null)
            {
                return Result.Failure<AuthenticationResult>(error);
            }

            var userCredentials = user.UserCredentials;

            var isVerified = _authServices.VerifyPasswordHash(
                                                 command.Password,
                                                 userCredentials.PasswordHash,
                                                 userCredentials.PasswordSalt);

            if (!isVerified) return Result.Failure<AuthenticationResult>(DomainErrors.UserCredentials.WrongCredentials);

            var userRoleIds = user.UserRoles
                                                .Select(ur => ur.RoleId)
                                                .ToList();
            var userRoles = await _roleRepository.GetAllRoleNamesByIdsAsync(userRoleIds, cancellationToken);
            
            var authenticationResult = _authServices
                .CreateToken(user.Id, user.UserName.Value, user.Email.Value, userRoles);

            userCredentials.AddOrUpdateRefreshToken(
                authenticationResult.RefreshToken,
                authenticationResult.RefreshTokenExpirationTimeUtc);

            _userCredentialsRepository.Update(userCredentials);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return authenticationResult;

        }

        private async Task<(User? user, Error? error)> GetUserAsync(LoginUserCommand command, CancellationToken cancellationToken)
        {
            User? user = null;

            if (!string.IsNullOrWhiteSpace(command.UserName))
            {
                var userName = UserName.Create(command.UserName);

                if (userName.IsFailure)
                {
                    {
                        return (user, userName.Error);
                    }
                }

                user = await _userRepository.GetByUserNameAsyncWithCredentialsAndRoles(userName.Value, cancellationToken);
            }
            else if (!string.IsNullOrWhiteSpace(command.Email))
            {
                var email = Email.Create(command.Email);

                if (email.IsFailure)
                {
                    {
                        return (user, email.Error);
                    }
                }
                
                user = await _userRepository.GetByEmailAsyncWithCredentialsAndRoles(email.Value, cancellationToken);
            }

            return (user, null);
        }
    }
}
