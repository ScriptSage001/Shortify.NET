using Shortify.NET.Applicaion.Abstractions;
using Shortify.NET.Applicaion.Abstractions.Repositories;
using Shortify.NET.Common.FunctionalTypes;
using Shortify.NET.Common.Messaging.Abstractions;
using Shortify.NET.Core;
using Shortify.NET.Core.Errors;

namespace Shortify.NET.Applicaion.Users.Commands.ResetPassword
{
    internal sealed class ResetPasswordCommandHandler(
        IUserCredentialsRepository userCredentialsRepository,
        IAuthServices authServices,
        IUnitOfWork unitOfWork) 
        : ICommandHandler<ResetPasswordCommand>
    {
        private readonly IUserCredentialsRepository _userCredentialsRepository = userCredentialsRepository;

        private readonly IAuthServices _authServices = authServices;

        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public async Task<Result> Handle(ResetPasswordCommand command, CancellationToken cancellationToken)
        {
            var userId = Guid.Parse(command.UserId);

            var userCreds = await _userCredentialsRepository
                                        .GetByUserIdAsync(
                                                userId, 
                                                cancellationToken);

            if (userCreds is null)
            {
                return Result.Failure(DomainErrors.User.UserNotFound);
            }

            var isVerified = _authServices
                                    .VerifyPasswordHash(
                                            command.OldPassword, 
                                            userCreds.PasswordHash, 
                                            userCreds.PasswordSalt);

            if (!isVerified) return Result.Failure(DomainErrors.UserCredentials.WrongCredentials);
            var (passwordHash, passwordSalt) = _authServices.CreatePasswordHashAndSalt(command.NewPassword);

            userCreds.UpdatePasswordHashAndSalt(passwordHash, passwordSalt);

            _userCredentialsRepository.Update(userCreds);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success();

        }
    }
}