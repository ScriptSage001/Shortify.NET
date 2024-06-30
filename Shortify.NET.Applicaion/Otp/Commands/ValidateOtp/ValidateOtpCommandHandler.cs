using Shortify.NET.Applicaion.Abstractions;
using Shortify.NET.Applicaion.Abstractions.Repositories;
using Shortify.NET.Common.FunctionalTypes;
using Shortify.NET.Common.Messaging.Abstractions;
using Shortify.NET.Core;

namespace Shortify.NET.Applicaion.Otp.Commands.ValidateOtp
{
    internal sealed class ValidateOtpCommandHandler(
        IOtpRepository otpRepository,
        IUnitOfWork unitOfWork,
        IAuthServices authService) 
        : ICommandHandler<ValidateOtpCommand, string>
    {
        private readonly IOtpRepository _otpRepository = otpRepository;

        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        private readonly IAuthServices _authService = authService;

        public async Task<Result<string>> Handle(ValidateOtpCommand command, CancellationToken cancellationToken = default)
        {

            var (otpId, otp) = await _otpRepository.GetLatestUnusedOtpAsync(command.Email, cancellationToken);

            if (otpId == Guid.Empty || string.IsNullOrEmpty(otp))
                return Result.Failure<string>(Error.NotFound("OTP.NotFound", "No Unused OTP Found!"));
            if (!command.Otp.Equals(otp))
                return Result.Failure<string>(Error.Unauthorized("OTP.Unauthorized", "Provided OTP is wrong!"));
            var token = _authService.GenerateValidateOtpToken(command.Email);
                    
            await _otpRepository.MarkOtpDetailAsUsed(otpId, DateTime.UtcNow, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return token;

        }
    }
}
