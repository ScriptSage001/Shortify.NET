using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using Shortify.NET.Common.Messaging.Abstractions;

namespace Shortify.NET.API.Controllers.V1
{
    /// <summary>
    /// Provides endpoints for URL shortening operations.
    /// </summary>
    [ApiVersion("1.0")]
    [Route("error/{statusCode}")]
    [Route("v{version:apiVersion}/error/{statusCode}")]
    [Tags("\u2757 Error Endpoints")]
    public class ErrorController(IApiService apiService) 
        : BaseApiController(apiService)
    {
        #region Public Endpoints

        /// <summary>
        /// Redirects an Error Response to a Static Error Page.
        /// </summary>
        /// <param name="statusCode"></param>
        /// <returns></returns>
        [HttpGet]
        public IActionResult HandleErrorCode(int statusCode)
        {
            if (statusCode is 410 or 404)
            {
                return PhysicalFile(
                    Path.Combine(
                        Directory.GetCurrentDirectory(), 
                        "wwwroot", 
                        $"{statusCode}.html"), 
                    "text/html");
            }
            return StatusCode(statusCode);
        } 
        
        #endregion
    }
}

