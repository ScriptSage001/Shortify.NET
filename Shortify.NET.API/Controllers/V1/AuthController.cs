﻿using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shortify.NET.API.Contracts;
using Shortify.NET.API.Mappers;
using Shortify.NET.Application.Token.Commands.RevokeToken;
using Shortify.NET.Common.Messaging.Abstractions;
using Shortify.NET.Core.Enums;

namespace Shortify.NET.API.Controllers.V1
{
    /// <summary>
    /// Provides authentication-related operations including user registration, login,
    /// password reset, and token management.
    /// </summary>
    /// <remarks>
    /// This controller handles various authentication and authorization tasks:
    /// - User Registration
    /// - User Login (via username/email or OTP)
    /// - Password Management (reset and forgot password scenarios)
    /// - Token Management (refresh and revoke tokens)
    /// - Access Token Generation using client credentials
    /// 
    /// Each endpoint is designed to interact with the appropriate service and return
    /// standardized responses.
    /// </remarks>
    [ApiVersion("1.0")]
    [Route("api/auth")]
    [Route("api/v{version:apiVersion}/auth")]
    [Tags("🔑 Authentication")]
    public class AuthController(IApiService apiService) 
        : BaseApiController(apiService)
    {
        private readonly MapperProfiles _mapper = new();

        #region Public Endpoints

        #region Register
            
        /// <summary>
        /// Registers a new user.
        /// </summary>
        /// <param name="request">The request containing user registration details.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A response indicating the result of the registration.</returns>
        /// <response code="201">The user was successfully registered.</response>
        /// <response code="400">The request is invalid.</response>
        /// <response code="409">The request is conflicting with existing User.</response>
        /// <response code="401">The request contains an invalid ValidateOtpToken.</response>
        [HttpPost("register")]
        [ProducesResponseType(typeof(RegisterUserResponse), statusCode: StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ProblemDetails), statusCode: StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), statusCode: StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(ProblemDetails), statusCode: StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> RegisterUser([FromBody] RegisterUserRequest request, CancellationToken cancellationToken)
        {
            var command = _mapper.RegisterUserRequestToCommand(request, nameof(Roles.Customer));

            var response = await _apiService.SendAsync(command, cancellationToken);

            return response.IsFailure ?
                    HandleFailure(response) :
                    Created(nameof(RegisterUser), _mapper.AuthResultToRegisterUserResponse(response.Value));
        }

        /// <summary>
        /// Registers a new admin user. 
        /// Only an Admin User can register another Admin User. 
        /// </summary>
        /// <param name="request">The request containing user registration details.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A response indicating the result of the registration.</returns>
        /// <response code="201">The user was successfully registered.</response>
        /// <response code="400">The request is invalid.</response>
        /// <response code="409">The request is conflicting with existing User.</response>
        /// <response code="401">The request contains an invalid ValidateOtpToken.</response>
        [Authorize(Roles = "Admin")]
        [HttpPost("register/admin")]
        [ProducesResponseType(typeof(RegisterUserResponse), statusCode: StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ProblemDetails), statusCode: StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), statusCode: StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(ProblemDetails), statusCode: StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> RegisterAdminUser([FromBody] RegisterUserRequest request, CancellationToken cancellationToken)
        {
            var command = _mapper.RegisterUserRequestToCommand(request, nameof(Roles.Admin));

            var response = await _apiService.SendAsync(command, cancellationToken);

            return response.IsFailure ?
                HandleFailure(response) :
                Created(nameof(RegisterUser), _mapper.AuthResultToRegisterUserResponse(response.Value));
        }
        
        #endregion

        #region Login

        /// <summary>
        /// Logs in a registered user using either username or email.
        /// </summary>
        /// <param name="request">The request containing login details.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A response indicating the result of the login attempt.</returns>
        /// <response code="200">The user was successfully logged in.</response>
        /// <response code="400">The request is invalid.</response>
        /// <response code="401">The login credentials are incorrect.</response>
        [HttpPost("login")]
        [ProducesResponseType(typeof(LoginUserResponse), statusCode: StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), statusCode: StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), statusCode: StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> LoginUser([FromBody] LoginUserRequest request, CancellationToken cancellationToken)
        {
            var command = _mapper.LoginUserRequestToCommand(request);

            var response = await _apiService.SendAsync(command, cancellationToken);

            return response.IsFailure ?
                    HandleFailure(response) :
                    Ok(_mapper.AuthResultToLoginUserResponse(response.Value));
        }

        /// <summary>
        /// Logs in a registered user using OTP.
        /// </summary>
        /// <param name="request">The request containing OTP login details.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A response indicating the result of the OTP login attempt.</returns>
        /// <response code="200">The user was successfully logged in using OTP.</response>
        /// <response code="400">The request is invalid.</response>
        /// <response code="401">The OTP is incorrect or expired.</response>
        [HttpPost("login/otp")]
        [ProducesResponseType(typeof(LoginUserResponse), statusCode: StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), statusCode: StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), statusCode: StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> LoginUsingOtp([FromBody] LoginUsingOtpRequest request, CancellationToken cancellationToken)
        {
            var command = _mapper.LoginUsingOtpRequestToCommand(request);

            var response = await _apiService.SendAsync(command, cancellationToken);

            return response.IsFailure ?
                    HandleFailure(response) :
                    Ok(_mapper.AuthResultToLoginUserResponse(response.Value));
        }

        #endregion

        #region Password

        /// <summary>
        /// Resets the password of a logged-in user.
        /// </summary>
        /// <param name="request">The request containing the new password details.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A response indicating the result of the password reset attempt.</returns>
        /// <response code="200">The password was successfully reset.</response>
        /// <response code="400">The request is invalid.</response>
        /// <response code="401">The user is not authenticated.</response>
        [Authorize]
        [HttpPut("password/reset")]
        [ProducesResponseType(typeof(string), statusCode: StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), statusCode: StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), statusCode: StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest request, CancellationToken cancellationToken = default)
        {
            var userId = GetUser();

            if (string.IsNullOrWhiteSpace(userId))
            {
                return HandleUnauthorizedRequest();
            }

            var command = _mapper.ResetPasswordRequestToCommand(request, userId);

            var response = await _apiService.SendAsync(command, cancellationToken);

            return response.IsFailure ?
                    HandleFailure(response) :
                    Ok("Password Changed Successfully!");

        }

        /// <summary>
        /// Resets the password using OTP (forgot password scenario).
        /// </summary>
        /// <param name="request">The request containing the OTP and new password details.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A response indicating the result of the password reset attempt.</returns>
        /// <response code="200">The password was successfully reset using OTP.</response>
        /// <response code="400">The request is invalid.</response>
        /// <response code="401">The OTP is incorrect or expired.</response>
        [HttpPut("password/reset/forgot")]
        [ProducesResponseType(typeof(string), statusCode: StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), statusCode: StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), statusCode: StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> ResetPasswordUsingOtp([FromBody] ResetPasswordUsingOtpRequest request, CancellationToken cancellationToken = default)
        {
            var command = _mapper.ResetPasswordUsingOtpRequestToCommand(request);

            var response = await _apiService.SendAsync(command, cancellationToken);

            return response.IsFailure ?
                    HandleFailure(response) :
                    Ok("Password Changed Successfully!");

        }

        #endregion

        #region Token

        #region Refresh Token

        /// <summary>
        /// Refreshes tokens.
        /// </summary>
        /// <param name="request">The request containing refresh token details.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A response indicating the result of the token refresh attempt.</returns>
        /// <response code="200">The tokens were successfully refreshed.</response>
        /// <response code="400">The request is invalid.</response>
        /// <response code="401">The refresh token is invalid or expired.</response>
        [HttpPost("token/refresh")]
        [ProducesResponseType(typeof(AuthenticationResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), statusCode: StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), statusCode: StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest request, CancellationToken cancellationToken = default)
        {
            var command = _mapper.RefreshTokenRequestToCommand(request);

            var response = await _apiService.SendAsync(command, cancellationToken);

            return response.IsFailure ?
                    HandleFailure(response)
                    : Ok(_mapper.AuthenticationResultToResponse(response.Value));
        }

        /// <summary>
        /// Revokes refresh tokens by user ID.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A response indicating the result of the token revocation attempt.</returns>
        /// <response code="200">The refresh tokens were successfully revoked.</response>
        /// <response code="404">The user is not found.</response>
        /// <response code="401">The user is not authenticated.</response>
        [Authorize]
        [HttpPut("token/refresh/revoke")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), statusCode: StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), statusCode: StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> RevokeRefreshToken(CancellationToken cancellationToken = default)
        {
            var userId = GetUser();

            if (string.IsNullOrWhiteSpace(userId))
            {
                return HandleUnauthorizedRequest();
            }

            var command = new RevokeRefreshTokenCommand(userId);

            var response = await _apiService.SendAsync(command, cancellationToken);

            return response.IsFailure ?
                    HandleFailure(response) :
                    Ok("Refresh Token Revoked Successfully!");
        }

        #endregion

        #region Access Token

        /// <summary>
        /// Generates access and refresh tokens using client secret.
        /// </summary>
        /// <param name="clientCredentials">The client credentials.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A response indicating the result of the token generation attempt.</returns>
        /// <response code="200">The tokens were successfully generated.</response>
        /// <response code="400">The request is invalid.</response>
        /// <response code="401">The client credentials are invalid.</response>
        [HttpPost("token")]
        [ProducesResponseType(typeof(AuthenticationResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), statusCode: StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), statusCode: StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetTokenByClientSecret([FromBody] ClientCredentials clientCredentials, CancellationToken cancellationToken = default)
        {
            var command = _mapper.ClientCredentialsToGenerateTokenByClientSecretCommand(clientCredentials);

            var response = await _apiService.SendAsync(command, cancellationToken);

            return response.IsFailure ?
                    HandleFailure(response) :
                    Ok(_mapper.AuthenticationResultToResponse(response.Value));
        }

        #endregion

        #endregion

        #endregion
    }
}
