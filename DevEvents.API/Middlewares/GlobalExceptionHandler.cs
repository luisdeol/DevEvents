using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace DevEvents.API.Middlewares
{
    public class GlobalExceptionHandler : IExceptionHandler
    {
        readonly ILogger<ExceptionHandlingMiddleware> _logger;
        readonly IWebHostEnvironment _env;

        public GlobalExceptionHandler(
            ILogger<ExceptionHandlingMiddleware> logger,
            IWebHostEnvironment env)
        {
            _logger = logger;
            _env = env;
        }

        public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
        {
            _logger.LogError(exception, "Error: {Message}", exception.Message);

            var statusCode = exception switch
            {
                ArgumentNullException => StatusCodes.Status400BadRequest,
                KeyNotFoundException => StatusCodes.Status404NotFound,
                _ => StatusCodes.Status500InternalServerError
            };

            var isDevelopment = _env.IsDevelopment();

            var details = isDevelopment ? exception.StackTrace : "";

            var problemDetails = new ProblemDetails
            {
                Status = statusCode,
                Title = "Unexpected Error.",
                Detail = details
            };

            httpContext.Response.StatusCode = problemDetails.Status.Value;

            await httpContext.Response.WriteAsJsonAsync(problemDetails);

            return true;
        }
    }
}
