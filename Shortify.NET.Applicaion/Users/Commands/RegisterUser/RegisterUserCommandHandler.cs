using Shortify.NET.Applicaion.Abstractions;
using Shortify.NET.Applicaion.Abstractions.Repositories;
using Shortify.NET.Applicaion.Shared.Models;
using Shortify.NET.Common.FunctionalTypes;
using Shortify.NET.Common.Messaging.Abstractions;
using Shortify.NET.Core;
using Shortify.NET.Core.Entites;
using Shortify.NET.Core.Errors;
using Shortify.NET.Core.ValueObjects;
using static Shortify.NET.Core.Entites.UserRole;

namespace Shortify.NET.Applicaion.Users.Commands.RegisterUser
{
    /// <summary>
    /// Handler for the <see cref="RegisterUserCommand"/>, to implement user registration.
    /// </summary>
    /// <param name="userRepository"></param>
    /// <param name="roleRepository"></param>
    /// <param name="unitOfWork"></param>
    /// <param name="authServices"></param>
    internal sealed class RegisterUserCommandHandler(
        IUserRepository userRepository,
        IRoleRepository roleRepository,
        IUnitOfWork unitOfWork,
        IAuthServices authServices) 
        : ICommandHandler<RegisterUserCommand, AuthenticationResult>
    {
        #region Variables
        
        private readonly IUserRepository _userRepository = userRepository;
        
        private readonly IRoleRepository _roleRepository = roleRepository;

        private readonly IAuthServices _authServices = authServices;

        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        #endregion
        
        #region Handle Method
        
        /// <summary>
        /// Handles the user registration command.
        /// </summary>
        /// <param name="command">The registration command.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The authentication result.</returns>
        public async Task<Result<AuthenticationResult>> Handle(RegisterUserCommand command, CancellationToken cancellationToken)
        {
            if (!_authServices.VerifyValidateOtpToken(command.Email, command.ValidateOtpToken))
            {
                return Result.Failure<AuthenticationResult>(Error.UnauthorizedRequest);
            }
            
            var userName = UserName.Create(command.UserName);
            var email = Email.Create(command.Email);
            if (!await IsUniqueUser(userName.Value, email.Value, cancellationToken))
            {
                return Result.Failure<AuthenticationResult>(DomainErrors.User.UserNameOrEmailAlreadyInUse);
            }

            var userId = Guid.NewGuid();
            var (passwordHash, passwordSalt) = _authServices.CreatePasswordHashAndSalt(command.Password);

            var newUserCredentials = UserCredentials.Create(
                userId: userId,
                passwordHash: passwordHash,
                passwordSalt: passwordSalt);
            
            var authResult = _authServices
                .CreateToken(
                    userId, 
                    userName.Value.Value, 
                    email.Value.Value,
                    [command.UserRole]);

            newUserCredentials
                .AddOrUpdateRefreshToken(
                    authResult.RefreshToken, 
                    authResult.RefreshTokenExpirationTimeUtc);
            
            var newUser = User.CreateUser(
                id: userId,
                userName: userName.Value,
                email: email.Value,
                userCredentials: newUserCredentials);

            await AddRoleToUser(newUser, command.UserRole, cancellationToken);
            
            _userRepository.Add(newUser);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return authResult;
        }
        
        #endregion
        
        #region Private Methods
        
        /// <summary>
        /// Checks to DB for uniqueness of UserName and Email.
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="email"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        private async Task<bool> IsUniqueUser(UserName userName, Email email, CancellationToken cancellationToken)
        {
            var isUserNameUnique = await _userRepository.IsUserNameUniqueAsync(userName, cancellationToken);
            var isEmailUnique = await _userRepository.IsEmailUniqueAsync(email, cancellationToken);
            return isUserNameUnique && isEmailUnique;
        }

        /// <summary>
        /// Adds the desired roles to the User.
        /// </summary>
        /// <param name="newUser"></param>
        /// <param name="roleName"></param>
        /// <param name="cancellationToken"></param>
        private async Task AddRoleToUser(User newUser, string roleName, CancellationToken cancellationToken)
        {
            var role = await _roleRepository.GetByNameAsync(roleName, cancellationToken);
            if (role is not null)
            {
                newUser.AddUserRole(Create(newUser.Id, role.Id));
            }
        }
        
        #endregion
    }
}