using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shortify.NET.API.Contracts;
using Shortify.NET.API.Mappers;
using Shortify.NET.Applicaion.Users.Queries.GetUserById;
using Shortify.NET.Common.Messaging.Abstractions;

namespace Shortify.NET.API.Controllers
{
    /// <summary>
    /// Provides endpoints for managing user-related operations.
    /// </summary>
    [Route("api/user")]
    [Authorize]
    [Tags("\ud83d\udc64 User Management")]
    public class UserController(IApiService apiService) 
        : BaseApiController(apiService)
    {
        private readonly MapperProfiles _mapper = new();

        #region Public Endpoints

        /// <summary>
        /// Retrieves the current user information.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The user information.</returns>
        /// <response code="200">Returns the user information.</response>
        /// <response code="400">If the request is invalid or the user identifier is not a valid GUID.</response>
        /// <response code="401">If the request is unauthorized.</response>
        /// <response code="404">If the user was not found.</response>
        /// <response code="500">If an error occurred while retrieving the user information.</response>
        [HttpGet]
        [ProducesResponseType(typeof(UserResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetCurrentUser(CancellationToken cancellationToken = default)
        {
            var id = GetUser();

            if (string.IsNullOrWhiteSpace(id))
            {
                return HandleUnauthorizedRequest();
            }

            var userId = Guid.Parse(id);

            var result = await _apiService.RequestAsync(new GetUserByIdQuery(userId), cancellationToken);

            return result.IsFailure ?
                        HandleFailure(result) :
                        Ok(_mapper.UserDtoToUserResponse(result.Value)); 
        }

        #endregion
    }
}
