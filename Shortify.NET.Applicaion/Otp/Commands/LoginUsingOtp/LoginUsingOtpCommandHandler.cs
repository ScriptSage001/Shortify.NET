using Shortify.NET.Applicaion.Abstractions;
using Shortify.NET.Applicaion.Abstractions.Repositories;
using Shortify.NET.Applicaion.Shared.Models;
using Shortify.NET.Common.FunctionalTypes;
using Shortify.NET.Common.Messaging.Abstractions;
using Shortify.NET.Core;
using Shortify.NET.Core.Errors;
using Email = Shortify.NET.Core.ValueObjects.Email;

namespace Shortify.NET.Applicaion.Otp.Commands.LoginUsingOtp
{
    internal sealed class LoginUsingOtpCommandHandler : ICommandHandler<LoginUsingOtpCommand, AuthenticationResult>
    {
        private readonly IUserRepository _userRepository;
        
        private readonly IOtpRepository _otpRepository;

        private readonly IUnitOfWork _unitOfWork;

        private readonly IAuthServices _authServices;

        private readonly IUserCredentialsRepository _userCredentialsRepository;

        public LoginUsingOtpCommandHandler(
            IUserRepository userRepository,
            IOtpRepository otpRepository,
            IUnitOfWork unitOfWork,
            IAuthServices authService,
            IUserCredentialsRepository userCredentialsRepository)
        {
            _userRepository = userRepository;
            _otpRepository = otpRepository;
            _unitOfWork = unitOfWork;
            _authServices = authService;
            _userCredentialsRepository = userCredentialsRepository;
        }

        public async Task<Result<AuthenticationResult>> Handle(LoginUsingOtpCommand command, CancellationToken cancellationToken)
        {
            Result<Email> email = Email.Create(command.Email);

            if (email.IsFailure)
            {
                return Result.Failure<AuthenticationResult>(email.Error);
            }

            var user = await _userRepository.GetByEmailAsyncWithCredentials(email.Value, cancellationToken);

            if (user == null)
            {
                return Result.Failure<AuthenticationResult>(DomainErrors.User.UserNotFound);
            }

            var isOtpValid = await IsOtpValid(command);

            if (isOtpValid)
            {
                AuthenticationResult authenticationResult = _authServices.CreateToken(user.Id, user.UserName.Value, user.Email.Value);

                user.UserCredentials.AddOrUpdateRefreshToken(
                                        authenticationResult.RefreshToken,
                                        authenticationResult.RefreshTokenExpirationTimeUtc);

                _userCredentialsRepository.Update(user.UserCredentials);

                await _unitOfWork.SaveChangesAsync(cancellationToken);

                return authenticationResult;
            }

            return Result.Failure<AuthenticationResult>(DomainErrors.Otp.Invalid);
        }

        private async Task<bool> IsOtpValid(LoginUsingOtpCommand command)
        {
            var (otpId, otp) = await _otpRepository.GetLatestUnusedOtpAsync(command.Email);

            if (otpId != Guid.Empty && !string.IsNullOrEmpty(otp))
            {
                if (command.Otp.Equals(otp))
                {
                    _otpRepository.MarkOtpDetailAsUsed(otpId, DateTime.UtcNow);

                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }
    }
}
