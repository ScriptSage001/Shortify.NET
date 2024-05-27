using Microsoft.AspNetCore.Mvc;
using Shortify.NET.Applicaion.Otp.Commands.SendOtp;
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

        #endregion
    }
}
