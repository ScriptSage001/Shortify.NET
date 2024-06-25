using Microsoft.AspNetCore.Mvc;
using Shortify.NET.API.Contracts;
using Shortify.NET.Applicaion.Otp.Commands.SendOtp;
using Shortify.NET.Applicaion.Otp.Commands.ValidateOtp;
using Shortify.NET.Common.Messaging.Abstractions;
using static Shortify.NET.Applicaion.Shared.Constant.EmailConstants;

namespace Shortify.NET.API.Controllers
{
    /// <summary>
    /// Provides endpoints for managing OTP (One Time Password) operations.
    /// </summary>
    [Route("api/otp")]
    [Tags("OTP Management")]
    public class OtpController(IApiService apiService) 
        : BaseApiController(apiService)
    {

        #region Public Endpoints

        #region Send OTP

        /// <summary>
        /// Sends an OTP for email verification.
        /// </summary>
        /// <param name="email">The email address to send the OTP to.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A response indicating whether the OTP was sent successfully.</returns>
        /// <response code="200">OTP sent successfully.</response>
        /// <response code="400">The request is invalid.</response>
        /// <response code="500">An error occurred while sending the OTP.</response> 
        [HttpPost("send/verify-email/{email}")]
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
        /// <response code="200">OTP sent successfully.</response>
        /// <response code="400">The request is invalid.</response>
        /// <response code="500">An error occurred while sending the OTP.</response>
        [HttpPost("send/login/{email}")]
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
        /// <response code="200">OTP sent successfully.</response>
        /// <response code="400">The request is invalid.</response>
        /// <response code="500">An error occurred while sending the OTP.</response>
        [HttpPost("send/forgot-password/{email}")]
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
        /// <response code="200">OTP validation successful.</response>
        /// <response code="400">The request is invalid.</response>
        /// <response code="500">An error occurred while validating the OTP.</response>
        [HttpPost("validate")]
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
