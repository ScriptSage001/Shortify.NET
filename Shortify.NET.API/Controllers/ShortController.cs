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
    public class ShortController : BaseApiController
    {
        private readonly MapperProfiles _mapper;

        public ShortController(IApiService apiService)
            : base(apiService)
        {
            _mapper = new MapperProfiles();
        }

        #region Public Endpoints

        /// <summary>
        /// To Shorte Any Url
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        [Route("api/shorten")]
        [ProducesResponseType(typeof(string), statusCode: StatusCodes.Status200OK)]
        [ProducesErrorResponseType(typeof(ProblemDetails))]
        public async Task<IActionResult> ShortenUrl([FromBody] ShortenUrlRequest request, CancellationToken cancellationToken = default)
        {
            if (request is null || string.IsNullOrWhiteSpace(request.Url))
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
                    Ok(response.Value.Value);
        }

        /// <summary>
        /// Endppoinf to rediect all the short url to Original Url
        /// </summary>
        /// <param name="code"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
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
        /// Get all Shortened Urls of the Current User
        /// </summary>
        /// <param name="cancellation"></param>
        /// <returns></returns>
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
        /// Get Shortened Url by the Id
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
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
        /// Get Shortened Url by the Code
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
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
