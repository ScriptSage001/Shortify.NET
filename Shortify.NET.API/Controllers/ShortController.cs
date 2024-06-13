using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shortify.NET.API.Contracts;
using Shortify.NET.API.Mappers;
using Shortify.NET.Applicaion.Url.Commands.ShortenUrl;
using Shortify.NET.Applicaion.Url.Queries.ShortenedUrl;
using Shortify.NET.Applicaion.Url.Queries.GetAllShortenedUrls;
using Shortify.NET.Common.FunctionalTypes;
using Shortify.NET.Common.Messaging.Abstractions;
using Shortify.NET.Applicaion.Url.Queries.GetOriginalUrl;

namespace Shortify.NET.API.Controllers
{
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
        [Authorize]
        [HttpPost]
        [Route("api/shorten")]
        [ProducesResponseType(typeof(string), statusCode: StatusCodes.Status201Created)]
        [ProducesErrorResponseType(typeof(ProblemDetails))]
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

            string userId = GetUser();

            if (string.IsNullOrWhiteSpace(userId))
            {
                return HandleUnauthorizedRequest();
            }

            ShortenUrlCommand command = _mapper.ShortenUrlRequestToCommand(request, userId, HttpContext.Request);

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
        [HttpGet]
        [Route("/{code}")]
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
        [Authorize]
        [HttpGet]
        [Route("api/shorten/getAll")]
        [ProducesResponseType(typeof(ShortenedUrlResponse), statusCode: StatusCodes.Status200OK)]
        [ProducesErrorResponseType(typeof(ProblemDetails))]
        public async Task<IActionResult> GetAllShortenedUrls(CancellationToken cancellationToken = default)
        {
            string userId = GetUser();

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
        /// Gets a shortened URL by its ID.
        /// </summary>
        /// <param name="id">The ID of the shortened URL.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The shortened URL.</returns>
        [Authorize]
        [HttpGet]
        [Route("api/shorten/{id}")]
        [ProducesResponseType(typeof(ShortenedUrlResponse), statusCode: StatusCodes.Status200OK)]
        [ProducesErrorResponseType(typeof(ProblemDetails))]
        public async Task<IActionResult> GetShortenedUrl(Guid id, CancellationToken cancellationToken = default)
        {
            var query = new GetShortenedUrlByIdQuery(id);

            var response = await _apiService.RequestAsync(query, cancellationToken);

            return response.IsFailure ?
                    HandleFailure(response) :
                    Ok(_mapper.ShortenedUrlDtoToResponse(response.Value));
        }

        /// <summary>
        /// Gets a shortened URL by its code.
        /// </summary>
        /// <param name="code">The code of the shortened URL.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The shortened URL.</returns>
        [Authorize]
        [HttpGet]
        [Route("api/shorten/get/{code}")]
        [ProducesResponseType(typeof(ShortenedUrlResponse), statusCode: StatusCodes.Status200OK)]
        [ProducesErrorResponseType(typeof(ProblemDetails))]
        public async Task<IActionResult> GetShortenedUrl(string code, CancellationToken cancellationToken = default)
        {
            var query = new GetShortenedUrlByCodeQuery(code);

            var response = await _apiService.RequestAsync(query, cancellationToken);

            return response.IsFailure ?
                    HandleFailure(response) :
                    Ok(_mapper.ShortenedUrlDtoToResponse(response.Value));
        }

        #endregion
    }
}
