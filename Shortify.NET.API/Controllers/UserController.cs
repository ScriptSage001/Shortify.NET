using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shortify.NET.API.Contracts;
using Shortify.NET.API.Mappers;
using Shortify.NET.Applicaion.Users.Queries.GetUserById;
using Shortify.NET.Common.Messaging.Abstractions;

namespace Shortify.NET.API.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    public class UserController(IApiService apiService) 
        : BaseApiController(apiService)
    {
        private readonly MapperProfiles _mapper = new MapperProfiles();

        #region Public Endpoints

        /// <summary>
        /// Gets the current user by UserId.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The user information.</returns>
        [HttpGet]
        [ProducesResponseType(typeof(UserResponse), StatusCodes.Status200OK)]
        [ProducesErrorResponseType(typeof(ProblemDetails))]
        public async Task<IActionResult> GetCurrentUser(CancellationToken cancellationToken = default)
        {
            string id = GetUser();

            if (string.IsNullOrWhiteSpace(id))
            {
                return HandleUnauthorizedRequest();
            }

            Guid userId = Guid.Parse(id);

            var result = await _apiService.RequestAsync(new GetUserByIdQuery(userId), cancellationToken);

            return result.IsFailure ?
                        HandleFailure(result) :
                        Ok(_mapper.UserDtoToUserResponse(result.Value)); 
        }

        #endregion
    }
}
