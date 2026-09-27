namespace NienLuan.Api.Common;

public class ApiException(int statusCode, string message, IReadOnlyDictionary<string, string[]>? errors = null)
    : Exception(message)
{
    public int StatusCode { get; } = statusCode;

    public IReadOnlyDictionary<string, string[]>? Errors { get; } = errors;
}

public static class ApiExceptionFactory
{
    public static ApiException BadRequest(string message) => new(StatusCodes.Status400BadRequest, message);

    public static ApiException Unauthorized(string message = "Unauthorized.") =>
        new(StatusCodes.Status401Unauthorized, message);

    public static ApiException Forbidden(string message = "Forbidden.") =>
        new(StatusCodes.Status403Forbidden, message);

    public static ApiException NotFound(string message = "Not found.") =>
        new(StatusCodes.Status404NotFound, message);

    public static ApiException Conflict(string message) => new(StatusCodes.Status409Conflict, message);
}
