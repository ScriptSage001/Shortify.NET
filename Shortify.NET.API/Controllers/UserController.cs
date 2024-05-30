using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shortify.NET.API.Mappers;
using Shortify.NET.Applicaion.Users.Queries.GetUserById;
using Shortify.NET.Common.Messaging.Abstractions;

namespace Shortify.NET.API.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    public class UserController : BaseApiController
    {
        private readonly MapperProfiles _mapper;

        public UserController(IApiService apiService) : base(apiService)
        {
            _mapper = new MapperProfiles();
        }

        #region Public Endpoints

        /// <summary>
        /// To Get a User by UserId
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> GetUser(string id, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return HandleNullOrEmptyRequest();
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
