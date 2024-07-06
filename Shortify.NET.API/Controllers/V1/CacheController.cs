using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shortify.NET.Applicaion.Cache.Commands.ClearAll;
using Shortify.NET.Applicaion.Cache.Commands.ClearByPrefix;
using Shortify.NET.Applicaion.Shared;
using Shortify.NET.Common.Messaging.Abstractions;

namespace Shortify.NET.API.Controllers.V1
{
    /// <summary>
    /// Provides endpoints for Cache Management.
    /// </summary>
    /// <param name="apiService"></param>
    [ApiVersion("1.0")]
    [Route("api/cache")]
    [Route("api/v{version:apiVersion}/cache")]
    [Tags("\ud83e\uddf9 Cache Management")]
    public class CacheController(IApiService apiService)
        : BaseApiController(apiService)
    {
        #region Public Endpoints

        /// <summary>
        /// Clears all cache entries.
        /// </summary>
        /// <remarks>
        /// This endpoint is restricted to users with the Admin role. It triggers a command to clear all
        /// cache entries in the application.
        /// </remarks>
        /// <param name="cancellationToken">A token to cancel the operation.</param>
        /// <returns>An <see cref="IActionResult"/> indicating the result of the operation.</returns>
        /// <response code="200">Cache Cleared Successfully.</response>
        /// <response code="401">Unauthorized access.</response>
        /// <response code="403">Forbidden access. Only users with Admin role can clear the cache.</response>
        /// <response code="500">Internal server error.</response>
        [Authorize(Roles = "Admin")]
        [HttpDelete("clear")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(string))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ProblemDetails))]
        [ProducesResponseType(StatusCodes.Status403Forbidden, Type = typeof(ProblemDetails))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ProblemDetails))]
        public async Task<IActionResult> ClearAll(CancellationToken cancellationToken = default)
        {
            var command = new ClearAllCacheCommand();
            var result = await _apiService.SendAsync(command, cancellationToken);
            
            return result.IsFailure ? 
                HandleFailure(result) : 
                Ok("Cache Cleared Successfully.");
        }

        /// <summary>
        /// Clears all original urls cache entries.
        /// </summary>
        /// <remarks>
        /// This endpoint is restricted to users with the Admin role. It triggers a command to clear all
        /// the original urls cache entries in the application.
        /// </remarks>
        /// <param name="cancellationToken">A token to cancel the operation.</param>
        /// <returns>An <see cref="IActionResult"/> indicating the result of the operation.</returns>
        /// <response code="200">Cache Cleared Successfully.</response>
        /// <response code="401">Unauthorized access.</response>
        /// <response code="403">Forbidden access. Only users with Admin role can clear the cache.</response>
        /// <response code="500">Internal server error.</response>
        [Authorize(Roles = "Admin")]
        [HttpDelete("clear/urls")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(string))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ProblemDetails))]
        [ProducesResponseType(StatusCodes.Status403Forbidden, Type = typeof(ProblemDetails))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ProblemDetails))]
        public async Task<IActionResult> ClearAllOriginalUrls(CancellationToken cancellationToken = default)
        {
            var command = new ClearCacheByPrefixCommand(Constant.Cache.Prefixes.OriginalUrls);
            var result = await _apiService.SendAsync(command, cancellationToken);
            
            return result.IsFailure ? 
                HandleFailure(result) : 
                Ok("All Original Urls are Cleared from Cache Successfully.");
        }
        
        #endregion
    }
}