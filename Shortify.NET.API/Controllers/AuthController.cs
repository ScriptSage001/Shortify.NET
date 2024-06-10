using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shortify.NET.API.Contracts;
using Shortify.NET.API.Mappers;
using Shortify.NET.Applicaion.Otp.Commands.LoginUsingOtp;
using Shortify.NET.Applicaion.Token.Commands.GetTokenByClientSecret;
using Shortify.NET.Applicaion.Token.Commands.RefreshToken;
using Shortify.NET.Applicaion.Token.Commands.RevokeToken;
using Shortify.NET.Applicaion.Users.Commands.ForgetPassword;
using Shortify.NET.Applicaion.Users.Commands.LoginUser;
using Shortify.NET.Applicaion.Users.Commands.RegisterUser;
using Shortify.NET.Applicaion.Users.Commands.ResetPassword;
using Shortify.NET.Common.Messaging.Abstractions;

namespace Shortify.NET.API.Controllers
{
    [Route("api/[controller]")]
    public class AuthController(IApiService apiService) 
        : BaseApiController(apiService)
    {
        private readonly MapperProfiles _mapper = new MapperProfiles();

        #region Public Endpoints

        #region Register
            
        /// <summary>
        /// Registers a new user.
        /// </summary>
        /// <param name="request">The request containing user registration details.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A response indicating the result of the registration.</returns>
        [HttpPost]
        [Route("register")]
        [ProducesResponseType(typeof(RegisterUserResponse), statusCode: StatusCodes.Status201Created)]
        [ProducesErrorResponseType(typeof(ProblemDetails))]
        public async Task<IActionResult> RegisterUser([FromBody] RegisterUserRequest request, CancellationToken cancellationToken)
        {
            RegisterUserCommand command = _mapper.RegisterUserRequestToCommand(request);

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
        [HttpPost]
        [Route("login")]
        [ProducesResponseType(typeof(LoginUserResponse), statusCode: StatusCodes.Status200OK)]
        [ProducesErrorResponseType(typeof(ProblemDetails))]
        public async Task<IActionResult> LoginUser([FromBody] LoginUserRequest request, CancellationToken cancellationToken)
        {
            LoginUserCommand command = _mapper.LoginUserRequestToCommand(request);

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
        [HttpPost]
        [Route("login/otp")]
        [ProducesResponseType(typeof(LoginUserResponse), statusCode: StatusCodes.Status200OK)]
        [ProducesErrorResponseType(typeof(ProblemDetails))]
        public async Task<IActionResult> LoginUsingOtp([FromBody] LoginUsingOtpRequest request, CancellationToken cancellationToken)
        {
            LoginUsingOtpCommand command = _mapper.LoginUsingOtpRequestToCommand(request);

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
        [Authorize]
        [HttpPut]
        [Route("password/reset")]
        [ProducesResponseType(typeof(string), statusCode: StatusCodes.Status200OK)]
        [ProducesErrorResponseType(typeof(ProblemDetails))]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest request, CancellationToken cancellationToken = default)
        {
            string userId = GetUser();

            if (string.IsNullOrWhiteSpace(userId))
            {
                return HandleUnauthorizedRequest();
            }

            ResetPasswordCommand command = _mapper.ResetPasswordRequestToCommand(request, userId);

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
        [HttpPut]
        [Route("password/reset/forgot")]
        [ProducesResponseType(typeof(string), statusCode: StatusCodes.Status200OK)]
        [ProducesErrorResponseType(typeof(ProblemDetails))]
        public async Task<IActionResult> ResetPasswordUsingOtp([FromBody] ResetPasswordUsingOtpRequest request, CancellationToken cancellationToken = default)
        {
            ResetPasswordUsingOtpCommand command = _mapper.ResetPasswordUsingOtpRequestToCommand(request);

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
        [HttpPost]
        [Route("token/refresh")]
        [ProducesResponseType(typeof(AuthenticationResponse), StatusCodes.Status200OK)]
        [ProducesErrorResponseType(typeof(ProblemDetails))]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest request, CancellationToken cancellationToken = default)
        {
            RefreshTokenCommand command = _mapper.RefreshTokenRequestToCommand(request);

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
        [Authorize]
        [HttpPut]
        [Route("token/refresh/revoke")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesErrorResponseType(typeof(ProblemDetails))]
        public async Task<IActionResult> RevokeRefreshToken(CancellationToken cancellationToken = default)
        {
            string userId = GetUser();

            if (string.IsNullOrWhiteSpace(userId))
            {
                return HandleUnauthorizedRequest();
            }

            RevokeRefreshTokenCommand command = new RevokeRefreshTokenCommand(userId);

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
        [HttpPost]
        [Route("token")]
        [ProducesResponseType(typeof(AuthenticationResponse), StatusCodes.Status200OK)]
        [ProducesErrorResponseType(typeof(ProblemDetails))]
        public async Task<IActionResult> GetTokenByClientSecret([FromBody] ClientCredentials clientCredentials, CancellationToken cancellationToken = default)
        {
            GenerateTokenByClientSecretCommand command = _mapper.ClientCredentialsToGenerateTokenByClientSecretCommand(clientCredentials);

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
