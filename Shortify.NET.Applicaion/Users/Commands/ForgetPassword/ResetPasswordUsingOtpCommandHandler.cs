using Shortify.NET.Applicaion.Abstractions;
using Shortify.NET.Applicaion.Abstractions.Repositories;
using Shortify.NET.Common.FunctionalTypes;
using Shortify.NET.Common.Messaging.Abstractions;
using Shortify.NET.Core;
using Shortify.NET.Core.Errors;
using Shortify.NET.Core.ValueObjects;

namespace Shortify.NET.Applicaion.Users.Commands.ForgetPassword
{
    internal sealed class ResetPasswordUsingOtpCommandHandler : ICommandHandler<ResetPasswordUsingOtpCommand>
    {
        private readonly IUserCredentialsRepository _userCredentialsRepository;

        private readonly IAuthServices _authServices;

        private readonly IUnitOfWork _unitOfWork;

        public ResetPasswordUsingOtpCommandHandler(
            IUserCredentialsRepository userCredentialsRepository,
            IAuthServices authServices,
            IUnitOfWork unitOfWork)
        {
            _userCredentialsRepository = userCredentialsRepository;
            _authServices = authServices;
            _unitOfWork = unitOfWork;
        }

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

            bool isVerified = _authServices.VerifyValidateOtpToken(command.Email, command.ValidateOtpToken);

            if (isVerified)
            {
                (byte[] passwordHash, byte[] passwordSalt) = _authServices.CreatePasswordHashAndSalt(command.NewPassword);

                userCreds.UpdatePasswordHashAndSalt(passwordHash, passwordSalt);

                _userCredentialsRepository.Update(userCreds);

                await _unitOfWork.SaveChangesAsync(cancellationToken);

                return Result.Success();
            }
            else
            {
                return Result.Failure(DomainErrors.UserCredentials.WrongCredentials);
            }
        }
    }
}
