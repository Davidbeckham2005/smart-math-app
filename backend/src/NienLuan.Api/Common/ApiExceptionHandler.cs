using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace NienLuan.Api.Common;

public class ApiExceptionHandler(IProblemDetailsService problemDetailsService, ILogger<ApiExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        var apiException = exception as ApiException;

        if (apiException is null)
        {
            logger.LogError(exception, "Unhandled exception for {Method} {Path}", httpContext.Request.Method, httpContext.Request.Path);
        }

        var statusCode = apiException?.StatusCode ?? StatusCodes.Status500InternalServerError;
        var message = apiException?.Message ?? "Đã xảy ra lỗi không mong muốn.";

        if (apiException?.Errors is { Count: > 0 } errors)
        {
            httpContext.Response.StatusCode = statusCode;
            return await problemDetailsService.TryWriteAsync(new ProblemDetailsContext
            {
                HttpContext = httpContext,
                ProblemDetails = new ValidationProblemDetails(errors.ToDictionary(e => e.Key, e => e.Value))
                {
                    Status = statusCode,
                    Title = message,
                },
            });
        }

        httpContext.Response.StatusCode = statusCode;

        return await problemDetailsService.TryWriteAsync(new ProblemDetailsContext
        {
            HttpContext = httpContext,
            ProblemDetails = new ProblemDetails
            {
                Status = statusCode,
                Title = message,
            },
        });
    }
}
