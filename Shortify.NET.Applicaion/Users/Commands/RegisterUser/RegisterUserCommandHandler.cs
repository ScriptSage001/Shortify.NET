using Shortify.NET.Applicaion.Abstractions;
using Shortify.NET.Applicaion.Abstractions.Repositories;
using Shortify.NET.Applicaion.Shared.Models;
using Shortify.NET.Common.FunctionalTypes;
using Shortify.NET.Common.Messaging.Abstractions;
using Shortify.NET.Core;
using Shortify.NET.Core.Entites;
using Shortify.NET.Core.Errors;
using Shortify.NET.Core.ValueObjects;

namespace Shortify.NET.Applicaion.Users.Commands.RegisterUser
{
    internal sealed class RegisterUserCommandHandler : ICommandHandler<RegisterUserCommand, AuthenticationResult>
    {
        private readonly IUserRepository _userRepository;

        private readonly IUserCredentialsRepository _userCredentialsRepository;

        private readonly IAuthServices _authServices;

        private readonly IUnitOfWork _unitOfWork;

        public RegisterUserCommandHandler(
            IUserRepository userRepository, 
            IUserCredentialsRepository userCredentialsRepository, 
            IUnitOfWork unitOfWork, 
            IAuthServices authServices)
        {
            _userRepository = userRepository;
            _userCredentialsRepository = userCredentialsRepository;
            _unitOfWork = unitOfWork;
            _authServices = authServices;
        }

        public async Task<Result<AuthenticationResult>> Handle(RegisterUserCommand command, CancellationToken cancellationToken)
        {
            Result<UserName> userName = UserName.Create(command.UserName);
            Result<Email> email = Email.Create(command.Email);

            bool isUserNameUnique = await _userRepository.IsUserNameUniqueAsync(userName.Value, cancellationToken);
            bool isEmailUnique = await _userRepository.IsEmailUniqueAsync(email.Value, cancellationToken);

            if (!isUserNameUnique)
            {
                return Result.Failure<AuthenticationResult>(DomainErrors.User.UserNameAlreadyInUse);
            }

            if (!isEmailUnique)
            {
                return Result.Failure<AuthenticationResult>(DomainErrors.User.EmailAlreadyInUse);
            }

            User newUser = User.CreateUser(
                                    userName: userName.Value,
                                    email: email.Value);

            _userRepository.Add(newUser);

            var (passwordHash, passwordSalt) = _authServices.CreatePasswordHashAndSalt(command.Password);

            UserCredentials newUserCredentials = UserCredentials.Create(
                                                                    userId: newUser.Id,
                                                                    passwordHash: passwordHash,
                                                                    passwordSalt: passwordSalt);

            AuthenticationResult authResult = _authServices.CreateToken(newUser.Id, email.Value.Value);

            newUserCredentials.AddOrUpdateRefreshToken(authResult.RefreshToken, authResult.RefreshTokenExpirationTimeUtc);

            _userCredentialsRepository.Add(newUserCredentials);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return authResult;
        }
    }
}
