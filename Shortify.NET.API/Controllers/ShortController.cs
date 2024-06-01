using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shortify.NET.API.Contracts;
using Shortify.NET.API.Mappers;
using Shortify.NET.Applicaion.Url.Commands.ShortenUrl;
using Shortify.NET.Applicaion.Url.Queries.ShortenedUrl;
using Shortify.NET.Common.FunctionalTypes;
using Shortify.NET.Common.Messaging.Abstractions;

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
        public async Task<IActionResult> ShortenUrl([FromBody] ShortenUrlRequest request, CancellationToken cancellationToken)
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

        [HttpGet]
        [Route("/{code}")]
        public async Task<IActionResult> RedirectUrl(string code, CancellationToken cancellationToken)
        {
            var query = new GetShortenedUrlQuery(code);

            var response = await _apiService.RequestAsync(query, cancellationToken);

            return response.IsFailure ?
                    HandleFailure(response) :
                    Redirect(response.Value);
        }

        #endregion
    }
}
