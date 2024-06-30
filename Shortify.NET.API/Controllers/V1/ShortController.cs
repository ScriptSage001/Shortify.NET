using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shortify.NET.API.Contracts;
using Shortify.NET.API.Mappers;
using Shortify.NET.Applicaion.Url.Queries.GetAllShortenedUrls;
using Shortify.NET.Applicaion.Url.Queries.GetOriginalUrl;
using Shortify.NET.Applicaion.Url.Queries.ShortenedUrl;
using Shortify.NET.Common.FunctionalTypes;
using Shortify.NET.Common.Messaging.Abstractions;

namespace Shortify.NET.API.Controllers.V1
{
    /// <summary>
    /// Provides endpoints for URL shortening operations.
    /// </summary>
    [ApiVersion("1.0")]
    [Route("api/shorten")]
    [Route("api/v{version:apiVersion}/shorten")]
    [Tags("\u2702\ufe0f URL Shortening")]
    public class ShortController(IApiService apiService) 
        : BaseApiController(apiService)
    {
        private readonly MapperProfiles _mapper = new();

        #region Public Endpoints

        /// <summary>
        /// Shortens a given URL.
        /// </summary> 
        /// <param name="request">The request containing the URL to shorten.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The shortened URL.</returns>
        /// <response code="201">URL shortened successfully.</response>
        /// <response code="400">The request is invalid.</response>
        /// <response code="401">Unauthorized request.</response>
        /// <response code="500">An error occurred while shortening the URL.</response>
        [Authorize]
        [HttpPost("")]
        [ProducesResponseType(typeof(string), statusCode: StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ShortenUrl([FromBody] ShortenUrlRequest request, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(request.Url))
            {
                return HandleNullOrEmptyRequest();
            }

            if(!Uri.TryCreate(request.Url, UriKind.Absolute, out _))
            {
                return HandleFailure(
                    Result.Failure(
                        Error.Validation(
                            "Error.ValidationError", 
                            "The specified URL is not valid.")));
            }

            var userId = GetUser();

            if (string.IsNullOrWhiteSpace(userId))
            {
                return HandleUnauthorizedRequest();
            }

            var command = _mapper.ShortenUrlRequestToCommand(request, userId, HttpContext.Request);

            var response = await _apiService.SendAsync(command, cancellationToken);

            return response.IsFailure ?
                    HandleFailure(response) :
                    Created(nameof(ShortenUrl), response.Value.Value);
        }

        /// <summary>
        /// Redirects the short URL to the original URL.
        /// </summary>
        /// <param name="code">The code of the short URL.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A redirection to the original URL.</returns>
        /// <response code="302">Redirection to the original URL.</response>
        /// <response code="400">The request is invalid.</response>
        /// <response code="404">The short URL was not found.</response>
        /// <response code="500">An error occurred while redirecting the URL.</response>
        [HttpGet]
        [Route("/{code}", Order = 1)]
        [ProducesResponseType(StatusCodes.Status302Found)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> RedirectUrl(string code, CancellationToken cancellationToken = default)
        {
            var query = new GetOriginalUrlQuery(code);

            var response = await _apiService.RequestAsync(query, cancellationToken);

            return response.IsFailure ?
                    HandleFailure(response) :
                    Redirect(response.Value);
        }

        /// <summary>
        /// Gets all shortened URLs of the current user.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A list of shortened URLs.</returns>
        /// <response code="200">Retrieved all shortened URLs successfully.</response>
        /// <response code="401">Unauthorized request.</response>
        /// <response code="500">An error occurred while retrieving the shortened URLs.</response>
        [Authorize]
        [HttpGet("getAll")]
        [ProducesResponseType(typeof(ShortenedUrlResponse), statusCode: StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAllShortenedUrls(CancellationToken cancellationToken = default)
        {
            var userId = GetUser();

            if (string.IsNullOrWhiteSpace(userId)) 
            {  
                return HandleUnauthorizedRequest(); 
            }

            var query = new GetAllShortenedUrlsQuery(userId);

            var response = await _apiService.RequestAsync(query, cancellationToken);

            return response.IsFailure ?
                    HandleFailure(response) :
                    Ok(_mapper.ShortenedUrlDtoListToResponseList(response.Value));
        }

        /// <summary>
        /// Gets a shortened URL by its identifier, which can be either a GUID or a code.
        /// </summary>
        /// <param name="identifier">The identifier of the shortened URL, either a GUID or a code.</param>
        /// <param name="isCode">Indicates whether the identifier is a code. If false, the identifier is treated as a GUID.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The shortened URL.</returns>
        /// <response code="200">Retrieved the shortened URL successfully.</response>
        /// <response code="400">The request is invalid. The identifier is not a valid GUID or Code.</response>
        /// <response code="401">Unauthorized request.</response>
        /// <response code="404">The shortened URL was not found.</response>
        /// <response code="500">An error occurred while retrieving the shortened URL.</response>
        [Authorize]
        [HttpGet("{identifier}")]
        [ProducesResponseType(typeof(ShortenedUrlResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetShortenedUrl(
            string identifier, 
            [FromQuery] bool isCode = false, 
            CancellationToken cancellationToken = default)
        {
            if (isCode)
            {
                var queryByCode = new GetShortenedUrlByCodeQuery(identifier);
                var responseByCode = await _apiService.RequestAsync(queryByCode, cancellationToken);

                return responseByCode.IsFailure ?
                        HandleFailure(responseByCode) :
                        Ok(_mapper.ShortenedUrlDtoToResponse(responseByCode.Value));
            }

            if (!Guid.TryParse(identifier, out var id))
            {
                return HandleFailure(
                    Result.Failure(
                        Error.Validation(
                            "Error.ValidationError",
                            "The specified identifier is not a valid GUID or Code.")));
            }

            var queryById = new GetShortenedUrlByIdQuery(id);
            var responseById = await _apiService.RequestAsync(queryById, cancellationToken);

            return responseById.IsFailure ?
                HandleFailure(responseById) :
                Ok(_mapper.ShortenedUrlDtoToResponse(responseById.Value));
        }
        
        #endregion
    }
}
