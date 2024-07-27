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
    /// <summary>
    /// Handler for the <see cref="LoginUserCommand"/>, to implement user login.
    /// </summary>
    /// <param name="userCredentialsRepository"></param>
    /// <param name="userRepository"></param>
    /// <param name="roleRepository"></param>
    /// <param name="authServices"></param>
    /// <param name="unitOfWork"></param>
    internal class LoginUserCommandHandler(
            IUserCredentialsRepository userCredentialsRepository,
            IUserRepository userRepository,
            IRoleRepository roleRepository,
            IAuthServices authServices,
            IUnitOfWork unitOfWork) 
        : ICommandHandler<LoginUserCommand, AuthenticationResult>
    {
        #region Variables
        
        private readonly IUserCredentialsRepository _userCredentialsRepository = userCredentialsRepository;
        private readonly IUserRepository _userRepository = userRepository;
        private readonly IRoleRepository _roleRepository = roleRepository;
        private readonly IAuthServices _authServices = authServices;
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        
        #endregion

        #region Handle Method
        
        /// <summary>
        /// Handles the user login command.
        /// </summary>
        /// <param name="command">The login command.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The authentication result.</returns>
        public async Task<Result<AuthenticationResult>> Handle(LoginUserCommand command, CancellationToken cancellationToken)
        {
            var (user, error) = await GetUserAsync(command, cancellationToken);
            if (user is null) return Result.Failure<AuthenticationResult>(error ?? DomainErrors.User.UserNotFound);

            var userCredentials = user.UserCredentials;
            var isVerified = _authServices.VerifyPasswordHash(
                                                 command.Password,
                                                 userCredentials.PasswordHash,
                                                 userCredentials.PasswordSalt);
            if (!isVerified) return Result.Failure<AuthenticationResult>(DomainErrors.UserCredentials.WrongCredentials);

            var userRoles = await GetUserRolesAsync(user, cancellationToken);
            var authenticationResult = CreateAuthenticationResult(user, userRoles);

            userCredentials.AddOrUpdateRefreshToken(
                authenticationResult.RefreshToken,
                authenticationResult.RefreshTokenExpirationTimeUtc);
            _userCredentialsRepository.Update(userCredentials);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return authenticationResult;
        }

        #endregion
        
        #region Private Methods
        
        /// <summary>
        /// Gets the user based on the userName or email.
        /// </summary>
        /// <param name="command">The login command.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The user and any error encountered.</returns>
        private async Task<(User? user, Error? error)> GetUserAsync(LoginUserCommand command, CancellationToken cancellationToken)
        {
            User? user = null;

            if (!string.IsNullOrWhiteSpace(command.UserName))
            {
                var userName = UserName.Create(command.UserName);
                if (userName.IsFailure) return (user, userName.Error);

                user = await _userRepository
                    .GetByUserNameAsyncWithCredentialsAndRoles(userName.Value, cancellationToken);
            }
            else if (!string.IsNullOrWhiteSpace(command.Email))
            {
                var email = Email.Create(command.Email);
                if (email.IsFailure) return (user, email.Error);
                
                user = await _userRepository.GetByEmailAsyncWithCredentialsAndRoles(email.Value, cancellationToken);
            }

            return (user, null);
        }
        
        /// <summary>
        /// Gets the roles of the user.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The list of user roles.</returns>
        private async Task<List<string>> GetUserRolesAsync(User user, CancellationToken cancellationToken)
        {
            var userRoleIds = user.UserRoles.Select(ur => ur.RoleId).ToList();
            return await _roleRepository.GetAllRoleNamesByIdsAsync(userRoleIds, cancellationToken);
        }

        /// <summary>
        /// Creates the authentication result.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <param name="userRoles">The list of user roles.</param>
        /// <returns>The authentication result.</returns>
        private AuthenticationResult CreateAuthenticationResult(User user, List<string> userRoles)
        {
            return _authServices.CreateToken(user.Id, user.UserName.Value, user.Email.Value, userRoles);
        }
        
        #endregion
    }
}