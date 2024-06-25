using Microsoft.AspNetCore.Mvc;
using Shortify.NET.Common.Messaging.Abstractions;

namespace Shortify.NET.API.Controllers
{
    /// <summary>
    /// Controller for application monitoring endpoints.
    /// Provides endpoints for checking the health status of the application.
    /// </summary>
    [Route("api/monitor")]
    [Tags("Application Monitoring")]
    public class MonitorController(IApiService apiService) 
        : BaseApiController(apiService)
    {

        #region Public Endpoints

        /// <summary>
        /// This API endpoint is commonly used to determine the health status or availability of the system and the services.
        /// It is a simple and lightweight endpoint designed to perform a quick health check of the application or infrastructure.
        /// </summary>
        /// <returns>A response indicating that the API is running.</returns>
        [HttpGet("health-check")]
        [ProducesResponseType(statusCode: StatusCodes.Status200OK)]
        public IActionResult HealthCheck()
        {
            return Ok("Health Check Passed!");
        }

        #endregion
    }
}