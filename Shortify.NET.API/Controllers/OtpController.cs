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
        /// Sends an OTP for email verification.
        /// </summary>
        /// <param name="email">The email address to send the OTP to.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A response indicating whether the OTP was sent successfully.</returns>
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

            var command = new SendOtpCommand(email, OtpType.VerifyEmail);

            var response = await _apiService.SendAsync(command, cancellationToken);

            return response.IsFailure ?
                        HandleFailure(response) :
                        Ok("Otp sent successfully.");
        }

        /// <summary>
        /// Sends an OTP for login.
        /// </summary>
        /// <param name="email">The email address to send the OTP to.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A response indicating whether the OTP was sent successfully.</returns>
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

            var command = new SendOtpCommand(email, OtpType.Login);

            var response = await _apiService.SendAsync(command, cancellationToken);

            return response.IsFailure ?
                        HandleFailure(response) :
                        Ok("Otp sent successfully.");
        }

        /// <summary>
        /// Sends an OTP to reset password.
        /// </summary>
        /// <param name="email">The email address to send the OTP to.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A response indicating whether the OTP was sent successfully.</returns>
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

            var command = new SendOtpCommand(email, OtpType.ResetPassword);

            var response = await _apiService.SendAsync(command, cancellationToken);

            return response.IsFailure ?
                        HandleFailure(response) :
                        Ok("Otp sent successfully.");
        }

        #endregion

        #region Validate OTP

        /// <summary>
        /// Validates the provided OTP.
        /// </summary>
        /// <param name="request">The request containing the email and OTP to validate.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A response indicating whether the OTP validation was successful.</returns>
        [HttpPost]
        [Route("validate")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ValidateOtp(ValidateOtpRequest request, CancellationToken cancellationToken = default)
        {
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
