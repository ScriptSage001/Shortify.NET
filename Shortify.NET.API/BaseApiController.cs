using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Shortify.NET.Common.FunctionalTypes;
using Shortify.NET.Common.Messaging.Abstractions;

namespace Shortify.NET.API
{
    /// <summary>
    /// Base ApiController with Generic Features
    /// </summary>
    [ApiController]
    public abstract class BaseApiController(IApiService apiService) 
        : ControllerBase
    {
        protected readonly IApiService _apiService = apiService;

        /// <summary>
        /// Handles Failure Scenarios to Generate
        /// Standard Machine-Readable ProblemDetails
        /// </summary>
        /// <param name="result"></param>
        /// <param name="isRedirectToErrorPage"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        protected IActionResult HandleFailure(Result result, bool isRedirectToErrorPage = false)
        {
            if (result.IsSuccess)
            {
                throw new InvalidOperationException();
            }
            if (result is IValidationResult validationResult)
            {
                var modelStateDictionary = new ModelStateDictionary();

                modelStateDictionary.AddModelError(result.Error.Code, result.Error.Message);

                foreach (var error in validationResult.Errors)
                {
                    modelStateDictionary.AddModelError(error.Code, error.Message);
                }

                return ValidationProblem(modelStateDictionary);
            }
            else
            {
                var error = result.Error;

                var statusCode = error.Type switch
                {
                    ErrorType.NotFound => StatusCodes.Status404NotFound,
                    ErrorType.Validation => StatusCodes.Status400BadRequest,
                    ErrorType.Conflict => StatusCodes.Status409Conflict,
                    ErrorType.Unauthorized => StatusCodes.Status401Unauthorized,
                    ErrorType.Gone => StatusCodes.Status410Gone,
                    _ => StatusCodes.Status500InternalServerError
                };

                if (isRedirectToErrorPage)
                {
                    return Redirect($"/error/{statusCode}"); 
                }

                return Problem(
                        statusCode: statusCode,
                        type: Enum.GetName(typeof(ErrorType), error.Type),
                        title: error.Code,
                        detail: error.Message);
            }
        }

        /// <summary>
        /// Handles Bad Request Situations 
        /// When the Request is Null or Empty
        /// </summary>
        /// <returns></returns>
        protected IActionResult HandleNullOrEmptyRequest()
        {
            var modelStateDictionary = new ModelStateDictionary();
            modelStateDictionary.AddModelError(Error.NullOrEmptyRequest.Code, Error.NullOrEmptyRequest.Message);

            return ValidationProblem(modelStateDictionary);
        }

        /// <summary>
        /// Handles Unauthorized Request
        /// </summary>
        /// <returns></returns>
        protected IActionResult HandleUnauthorizedRequest()
        {
            var modelStateDictionary = new ModelStateDictionary();
            modelStateDictionary.AddModelError(Error.UnauthorizedRequest.Code, Error.UnauthorizedRequest.Message);

            return ValidationProblem(modelStateDictionary);
        }

        /// <summary>
        /// Gets the userId from the Tokens claim
        /// </summary>
        /// <returns></returns>
        protected string GetUser()
        {
            var userIdClaims = User
                                        .Claims
                                        .FirstOrDefault(c => 
                                                c.Type.Equals("UserId", StringComparison.OrdinalIgnoreCase));

            return userIdClaims is null ? string.Empty : userIdClaims.Value;
        }
    }
}
