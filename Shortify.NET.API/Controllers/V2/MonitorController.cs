using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using Shortify.NET.Common.Messaging.Abstractions;

namespace Shortify.NET.API.Controllers.V2
{
    /// <summary>
    /// This is a sample controller to test API V2
    /// </summary>
    [ApiVersion("2.0")]
    [Route("api/v{version:apiVersion}/monitor")]
    [Tags("📊 Application Monitoring")]
    public class MonitorController(IApiService apiService)
        : BaseApiController(apiService)
    {
        #region Public Endpoints

        /// <summary>
        /// This is a sample endpoint to test API V2
        /// </summary>
        /// <returns>A response indicating that the API is running.</returns>
        [HttpGet("health-check")]
        [ProducesResponseType(statusCode: StatusCodes.Status200OK)]
        public IActionResult HealthCheck()
        {
            return Ok("Health Check Passed From API Version 2!");
        }

        #endregion
    }
}
