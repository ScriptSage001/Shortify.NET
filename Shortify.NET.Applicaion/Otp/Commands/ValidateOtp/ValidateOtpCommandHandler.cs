using Shortify.NET.Applicaion.Abstractions;
using Shortify.NET.Applicaion.Abstractions.Repositories;
using Shortify.NET.Common.FunctionalTypes;
using Shortify.NET.Common.Messaging.Abstractions;
using Shortify.NET.Core;

namespace Shortify.NET.Applicaion.Otp.Commands.ValidateOtp
{
    internal sealed class ValidateOtpCommandHandler : ICommandHandler<ValidateOtpCommand, string>
    {
        private readonly IOtpRepository _otpRepository;

        private readonly IUnitOfWork _unitOfWork;

        private readonly IAuthServices _authService;

        public ValidateOtpCommandHandler(
            IOtpRepository otpRepository, 
            IUnitOfWork unitOfWork, 
            IAuthServices authService)
        {
            _otpRepository = otpRepository;
            _unitOfWork = unitOfWork;
            _authService = authService;
        }

        public async Task<Result<string>> Handle(ValidateOtpCommand command, CancellationToken cancellationToken = default)
        {

            var (otpId, otp) = await _otpRepository.GetLatestUnusedOtpAsync(command.Email);

            if (otpId != Guid.Empty && !string.IsNullOrEmpty(otp))
            {
                if (command.Otp.Equals(otp))
                {
                    var token = _authService.GenerateValidateOtpToken(command.Email);
                    
                    await _otpRepository.MarkOtpDetailAsUsed(otpId, DateTime.UtcNow, cancellationToken);
                    await _unitOfWork.SaveChangesAsync(cancellationToken);

                    return token;
                }
                else
                {
                    return Result.Failure<string>(Error.Unauthorized("OTP.Unauthorized", "Provided OTP is wrong!"));
                }
            }
            else
            {
                return Result.Failure<string>(Error.NotFound("OTP.NotFound", "No Unused OTP Found!"));
            }
        }
    }
}
