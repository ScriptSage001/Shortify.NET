using Shortify.NET.Application.Abstractions;
using Shortify.NET.Application.Abstractions.Repositories;
using Shortify.NET.Common.FunctionalTypes;
using Shortify.NET.Common.Messaging.Abstractions;
using Shortify.NET.Core;
using Shortify.NET.Core.Errors;
using Shortify.NET.Core.ValueObjects;

namespace Shortify.NET.Application.Users.Commands.ForgetPassword
{
    internal sealed class ResetPasswordUsingOtpCommandHandler(
        IUserCredentialsRepository userCredentialsRepository,
        IAuthServices authServices,
        IUnitOfWork unitOfWork) 
        : ICommandHandler<ResetPasswordUsingOtpCommand>
    {
        private readonly IUserCredentialsRepository _userCredentialsRepository = userCredentialsRepository;

        private readonly IAuthServices _authServices = authServices;

        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public async Task<Result> Handle(ResetPasswordUsingOtpCommand command, CancellationToken cancellationToken)
        {
            var email = Email.Create(command.Email);

            if (email.IsFailure)
            {
                return Result.Failure(email.Error);
            }

            var userCreds = await _userCredentialsRepository.GetByEmailAsync(email.Value, cancellationToken);

            if (userCreds is null)
            {
                return Result.Failure(DomainErrors.User.UserNotFound);
            }

            var isVerified = _authServices.VerifyValidateOtpToken(command.Email, command.ValidateOtpToken);

            if (!isVerified) return Result.Failure(DomainErrors.UserCredentials.WrongCredentials);
            var (passwordHash, passwordSalt) = _authServices.CreatePasswordHashAndSalt(command.NewPassword);

            userCreds.UpdatePasswordHashAndSalt(passwordHash, passwordSalt);

            _userCredentialsRepository.Update(userCreds);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success();

        }
    }
}
