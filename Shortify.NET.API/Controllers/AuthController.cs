using Microsoft.AspNetCore.Mvc;
using Shortify.NET.API.Contracts;
using Shortify.NET.Applicaion.Users.Commands.LoginUser;
using Shortify.NET.Applicaion.Users.Commands.RegisterUser;
using Shortify.NET.Common.Messaging.Abstractions;

namespace Shortify.NET.API.Controllers
{
    [Route("api/[controller]")]
    public class AuthController : BaseApiController
    {
        public AuthController(IApiService apiService) : base(apiService)
        {
        }

        #region Public Endpoints

        /// <summary>
        /// To Register a new User
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("register")]
        [ProducesResponseType(typeof(RegisterUserResponse), statusCode: StatusCodes.Status200OK)]
        [ProducesErrorResponseType(typeof(ProblemDetails))]
        public async Task<IActionResult> RegisterUser([FromBody] RegisterUserRequest request, CancellationToken cancellationToken)
        {
            if (request is null)
            {
                return HandleNullOrEmptyRequest();
            }

            // Auto Map - ToDo
            var command = new RegisterUserCommand(
                UserName: request.UserName,
                Email: request.Email,
                Password: request.Password
                );

            var response = await _apiService.SendAsync(command, cancellationToken);

            return response.IsFailure ?
                    HandleFailure(response) :
                    Ok(new RegisterUserResponse(
                        UserId: response.Value.UserId,
                        AccessToken: response.Value.AccessToken,
                        RefreshToken: response.Value.RefreshToken,
                        RefreshTokenExpirationTimeUtc: response.Value.RefreshTokenExpirationTimeUtc)); // Auto Map - ToDo
        }

        /// <summary>
        /// To Login a Registered User
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("login")]
        [ProducesResponseType(typeof(LoginUserResponse), statusCode: StatusCodes.Status200OK)]
        [ProducesErrorResponseType(typeof(ProblemDetails))]
        public async Task<IActionResult> LoginUser([FromBody] LoginUserRequest request, CancellationToken cancellationToken)
        {
            if (request is null)
            {
                return HandleNullOrEmptyRequest();
            }

            var command = new LoginUserCommand(
                                    UserName: request.UserName,
                                    Password: request.Password);

            var response = await _apiService.SendAsync(command, cancellationToken);

            return response.IsFailure ?
                    HandleFailure(response) :
                    Ok(new LoginUserResponse(
                        UserId: response.Value.UserId,
                        AccessToken: response.Value.AccessToken,
                        RefreshToken: response.Value.RefreshToken,
                        RefreshTokenExpirationTimeUtc: response.Value.RefreshTokenExpirationTimeUtc)); // Auto Map - ToDo
        }

        /// <summary>
        /// To Login a Registered User using OTP
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("login/otp")]
        [ProducesResponseType(typeof(LoginUserResponse), statusCode: StatusCodes.Status200OK)]
        [ProducesErrorResponseType(typeof(ProblemDetails))]
        public async Task<IActionResult> LoginUsingOtp([FromBody] LoginUsingOtpRequest request, CancellationToken cancellationToken)
        {
            if (request is null)
            {
                return HandleNullOrEmptyRequest();
            }

            return Ok();
        }

        #endregion
    }
}
