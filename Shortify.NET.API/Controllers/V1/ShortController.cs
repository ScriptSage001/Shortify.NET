using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shortify.NET.API.Contracts;
using Shortify.NET.API.Mappers;
using Shortify.NET.Application.Url.Commands.DeleteUrl;
using Shortify.NET.Application.Url.Queries.GetAllShortenedUrls;
using Shortify.NET.Application.Url.Queries.GetOriginalUrl;
using Shortify.NET.Application.Url.Queries.ShortenedUrl;
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
        public async Task<IActionResult> ShortenUrl(
            [FromBody] ShortenUrlRequest request, 
            CancellationToken cancellationToken = default)
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
                    HandleFailure(response, true) :
                    Redirect(response.Value);
        }

        /// <summary>
        /// Gets shortened URLs of the current user.
        /// Search the URLs based on the Title or any Tag or a fraction of them.
        /// Sort by Title, CreatedOn, UpdatedOn in Ascending or Descending Order. Default sort is Ascending on Id.
        /// Paginated Response with Total Count.
        /// </summary>
        /// <param name="searchTerm">Search term can be the Title or any Tag or a fraction of them. Nullable.</param>
        /// <param name="sortColumn">Column to Sort the response on. Nullable. Default sort will be on Id.</param>
        /// <param name="sortOrder">Can be "desc" or null. Default is ascending.</param>
        /// <param name="page">Page No. Mandatory.</param>
        /// <param name="pageSize">Page Size. Mandatory.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A paged list of shortened URLs.</returns>
        /// <response code="200">Retrieved all shortened URLs successfully.</response>
        /// <response code="401">Unauthorized request.</response>
        /// <response code="500">An error occurred while retrieving the shortened URLs.</response>
        [Authorize]
        [HttpGet()]
        [ProducesResponseType(typeof(PagedList<ShortenedUrlResponse>), statusCode: StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetShortenedUrls(
            string? searchTerm,
            string? sortColumn,
            string? sortOrder,
            int page,
            int pageSize,
            CancellationToken cancellationToken = default)
        {
            var userId = GetUser();

            if (string.IsNullOrWhiteSpace(userId)) 
            {  
                return HandleUnauthorizedRequest(); 
            }

            var query = new GetShortenedUrlsQuery(
                UserId: userId,
                SearchTerm: searchTerm,
                SortColumn: sortColumn,
                SortOrder: sortOrder,
                Page: page,
                PageSize: pageSize);

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
        
        /// <summary>
        /// Deletes a shortened URL by its identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the shortened URL to delete.</param>
        /// <param name="cancellationToken">A token to cancel the operation.</param>
        /// <returns>
        /// An <see cref="IActionResult"/> indicating the result of the delete operation.
        /// If successful, returns a 200 OK with a success message.
        /// If the operation fails, returns the appropriate error response.
        /// </returns>
        /// <response code="200">If the shortened URL was deleted successfully.</response>
        /// <response code="400">If the request is invalid.</response>
        /// <response code="404">If the shortened URL with the specified ID was not found.</response>
        /// <response code="500">If an internal server error occurs.</response>
        [Authorize]
        [HttpDelete("{id:guid}")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteShortenedUrl(
            Guid id,
            CancellationToken cancellationToken = default)
        {
            var command = new DeleteShortenedUrlByIdCommand(id);
            var response = await _apiService.SendAsync(command, cancellationToken);

            return response.IsFailure ? 
                HandleFailure(response) : 
                Ok("Shortened URL deleted successfully!");
        }

        /// <summary>
        /// Updates Title and Tags of a shortened URL by its identifier.
        /// </summary>
        /// <param name="request">Update shortened URL request having identifier, title and tags.</param>
        /// <param name="cancellationToken">A token to cancel the operation.</param>
        /// <returns>
        /// An <see cref="IActionResult"/> indicating the result of the delete operation.
        /// If successful, returns a 200 OK with a success message.
        /// If the operation fails, returns the appropriate error response.
        /// </returns>
        /// <response code="200">If the shortened URL was updated successfully.</response>
        /// <response code="400">If the request is invalid.</response>
        /// <response code="404">If the shortened URL with the specified ID was not found.</response>
        /// <response code="500">If an internal server error occurs.</response>
        [Authorize]
        [HttpPut("update")]
        [ProducesResponseType(typeof(ShortenedUrlResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateShortenedUrl(
            [FromBody] UpdateShortenedUrlRequest request, 
            CancellationToken cancellationToken = default)
        {
            var command = _mapper.UpdateShortenedUrlRequestToCommand(request);
            var response = await _apiService.SendAsync(command, cancellationToken);
            
            return response.IsFailure ?
                HandleFailure(response) :
                Ok(_mapper.ShortenedUrlDtoToResponse(response.Value));
        }
        
        #endregion
    }
}
