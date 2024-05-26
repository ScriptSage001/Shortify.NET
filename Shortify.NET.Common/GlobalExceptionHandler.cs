using FluentValidation;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Shortify.NET.Common
{
    /// <summary>
    /// Common Class for Global Exception Handling
    /// </summary>
    public class GlobalExceptionHandler : IExceptionHandler
    {
        public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
        {
            int status = StatusCodes.Status500InternalServerError;

            if (exception is ValidationException)
            {
                status = StatusCodes.Status400BadRequest;
            }

            var response = new ProblemDetails()
            {
                Status = status,
                Title = exception.Message,
                Type = exception.GetType().ToString(),
                Detail = exception.StackTrace
            };

            httpContext.Response.StatusCode = status;
            await httpContext.Response.WriteAsJsonAsync(response, cancellationToken);

            return true;
        }
    }
}
