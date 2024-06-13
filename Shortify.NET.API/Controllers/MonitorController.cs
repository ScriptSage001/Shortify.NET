using Microsoft.AspNetCore.Mvc;
using Shortify.NET.Common.Messaging.Abstractions;

namespace Shortify.NET.API.Controllers
{
    [Route("api/[controller]")]
    public class MonitorController(IApiService apiService) 
        : BaseApiController(apiService)
    {

        #region Public Endpoints

        /// <summary>
        /// Checks the health status of the API.
        /// </summary>
        /// <returns>A response indicating that the API is running.</returns>
        [HttpGet]
        [Route("keepAlive")]
        [ProducesResponseType(statusCode: StatusCodes.Status200OK)]
        public IActionResult KeepAlive()
        {
            return Ok();
        }

        #endregion
    }
}
