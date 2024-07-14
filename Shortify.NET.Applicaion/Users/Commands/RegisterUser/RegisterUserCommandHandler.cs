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
    internal sealed class RegisterUserCommandHandler(
        IUserRepository userRepository,
        IRoleRepository roleRepository,
        IUnitOfWork unitOfWork,
        IAuthServices authServices) 
        : ICommandHandler<RegisterUserCommand, AuthenticationResult>
    {
        private readonly IUserRepository _userRepository = userRepository;
        
        private readonly IRoleRepository _roleRepository = roleRepository;

        private readonly IAuthServices _authServices = authServices;

        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public async Task<Result<AuthenticationResult>> Handle(RegisterUserCommand command, CancellationToken cancellationToken)
        {
            var isVerified = _authServices.VerifyValidateOtpToken(command.Email, command.ValidateOtpToken);

            if (!isVerified) return Result.Failure<AuthenticationResult>(Error.UnauthorizedRequest);
            var userName = UserName.Create(command.UserName);
            var email = Email.Create(command.Email);

            var isUserNameUnique = await _userRepository.IsUserNameUniqueAsync(userName.Value, cancellationToken);
            var isEmailUnique = await _userRepository.IsEmailUniqueAsync(email.Value, cancellationToken);

            if (!isUserNameUnique)
            {
                return Result.Failure<AuthenticationResult>(DomainErrors.User.UserNameAlreadyInUse);
            }

            if (!isEmailUnique)
            {
                return Result.Failure<AuthenticationResult>(DomainErrors.User.EmailAlreadyInUse);
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

            var role = await _roleRepository
                .GetByNameAsync(command.UserRole, cancellationToken);
            if (role is not null)
            {
                newUser.AddUserRole(Create(userId, role.Id));
            }
            
            _userRepository.Add(newUser);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return authResult;
        }
    }
}