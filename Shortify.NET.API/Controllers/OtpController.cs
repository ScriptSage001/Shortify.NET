using Microsoft.AspNetCore.Mvc;
using Shortify.NET.API.Contracts;
using Shortify.NET.Applicaion.Otp.Commands.SendOtp;
using Shortify.NET.Applicaion.Otp.Commands.ValidateOtp;
using Shortify.NET.Common.Messaging.Abstractions;
using static Shortify.NET.Applicaion.Shared.Constant.EmailConstants;

namespace Shortify.NET.API.Controllers
{
    [Route("api/[controller]")]
    public class OtpController : BaseApiController
    {
        public OtpController(IApiService apiService) : base(apiService)
        {
        }

        #region Public Endpoints

        #region Send OTP

        /// <summary>
        /// To Send Otp For Email Verification
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("send/verify-email/{email}")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> SendOtpForEmailVerification(string email, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                return HandleNullOrEmptyRequest();
            }

            var command = new SendOtpCommand(email, OTPType.VerifyEmail);

            var response = await _apiService.SendAsync(command, cancellationToken);

            return response.IsFailure ?
                        HandleFailure(response) :
                        Ok("Otp sent successfully.");
        }

        /// <summary>
        /// To Send Otp For Login
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("send/login/{email}")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> SendOtpForLogin(string email, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                return HandleNullOrEmptyRequest();
            }

            var command = new SendOtpCommand(email, OTPType.Login);

            var response = await _apiService.SendAsync(command, cancellationToken);

            return response.IsFailure ?
                        HandleFailure(response) :
                        Ok("Otp sent successfully.");
        }

        /// <summary>
        /// To Send Otp To Reset Password
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("send/forgot-password/{email}")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> SendOtpToResetPassword(string email, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                return HandleNullOrEmptyRequest();
            }

            var command = new SendOtpCommand(email, OTPType.ResetPassword);

            var response = await _apiService.SendAsync(command, cancellationToken);

            return response.IsFailure ?
                        HandleFailure(response) :
                        Ok("Otp sent successfully.");
        }

        #endregion

        #region Validate OTP

        [HttpPost]
        [Route("validate")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ValidateOtp(ValidateOtpRequest request, CancellationToken cancellationToken = default)
        {
            if (request is null )
            {
                return HandleNullOrEmptyRequest();
            }

            var command = new ValidateOtpCommand(request.Email, request.Otp);

            var response = await _apiService.SendAsync(command, cancellationToken);

            return response.IsFailure ?
                    HandleFailure(response) :
                    Ok(response.Value);
        }

        #endregion

        #endregion
    }
}
