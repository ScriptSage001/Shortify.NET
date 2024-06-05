using Microsoft.AspNetCore.Mvc;
using Shortify.NET.Common.Messaging.Abstractions;

namespace Shortify.NET.API.Controllers
{
    [Route("api/[controller]")]
    public class MonitorController : BaseApiController
    {
        public MonitorController(IApiService apiService) 
            : base(apiService)
        {
        }

        #region Public Endpoints

        /// <summary>
        /// To 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("keepAlive")]
        [ProducesResponseType(statusCode: StatusCodes.Status200OK)]
        public async Task<IActionResult> KeepAlive()
        {
            await Task.Delay(1);

            return Ok();
        }

        #endregion
    }
}
