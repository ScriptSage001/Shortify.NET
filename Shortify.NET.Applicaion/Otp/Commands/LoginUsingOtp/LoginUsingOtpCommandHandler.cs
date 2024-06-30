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
    internal sealed class LoginUsingOtpCommandHandler(
        IUserRepository userRepository,
        IOtpRepository otpRepository,
        IUnitOfWork unitOfWork,
        IAuthServices authService,
        IUserCredentialsRepository userCredentialsRepository) 
        : ICommandHandler<LoginUsingOtpCommand, AuthenticationResult>
    {
        private readonly IUserRepository _userRepository = userRepository;
        
        private readonly IOtpRepository _otpRepository = otpRepository;

        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        private readonly IAuthServices _authServices = authService;

        private readonly IUserCredentialsRepository _userCredentialsRepository = userCredentialsRepository;

        public async Task<Result<AuthenticationResult>> Handle(LoginUsingOtpCommand command, CancellationToken cancellationToken = default)
        {
            var email = Email.Create(command.Email);

            if (email.IsFailure)
            {
                return Result.Failure<AuthenticationResult>(email.Error);
            }

            var user = await _userRepository.GetByEmailAsyncWithCredentials(email.Value, cancellationToken);

            if (user is null)
            {
                return Result.Failure<AuthenticationResult>(DomainErrors.User.UserNotFound);
            }

            var isOtpValid = await IsOtpValid(command, cancellationToken);

            if (!isOtpValid) return Result.Failure<AuthenticationResult>(DomainErrors.Otp.Invalid);
            var authenticationResult = _authServices.CreateToken(user.Id, user.UserName.Value, user.Email.Value);

            user.UserCredentials.AddOrUpdateRefreshToken(
                authenticationResult.RefreshToken,
                authenticationResult.RefreshTokenExpirationTimeUtc);

            _userCredentialsRepository.Update(user.UserCredentials);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return authenticationResult;

        }

        private async Task<bool> IsOtpValid(LoginUsingOtpCommand command, CancellationToken cancellationToken = default)
        {
            var (otpId, otp) = await _otpRepository.GetLatestUnusedOtpAsync(command.Email, cancellationToken);

            if (otpId == Guid.Empty || string.IsNullOrEmpty(otp)) return false;
            if (!command.Otp.Equals(otp)) return false;
            await _otpRepository.MarkOtpDetailAsUsed(otpId, DateTime.UtcNow, cancellationToken);

            return true;

        }
    }
}
