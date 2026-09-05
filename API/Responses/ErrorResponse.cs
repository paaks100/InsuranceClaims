using Microsoft.AspNetCore.Mvc;

namespace InsuranceClaims.API.Responses;

public class ErrorResponse
{
    public string? Type { get; init; }
    public string Message { get; init; } = null!;
    public int Status { get; init; }
    public IEnumerable<string>? Errors { get; init; }
    public string? TraceId { get; init; }

    public static ErrorResponse Error(
        string message,
        int status,
        IEnumerable<string>? errors = null,
        string? traceId = null,
        string? type = null
    ) =>
        new()
        {
            Type = type,
            Message = message,
            Status = status,
            Errors = errors,
            TraceId = traceId,
        };

    public static ErrorResponse ValidationError(
        IEnumerable<string> errors,
        string? traceId = null
    ) =>
        new()
        {
            Type = "https://tools.ietf.org/html/rfc9110#section-15.5.1",
            Message = "One or more validation errors occurred.",
            Status = StatusCodes.Status400BadRequest,
            Errors = errors,
            TraceId = traceId,
        };

    public static ErrorResponse NotFound(string message) =>
        new()
        {
            Type = "https://tools.ietf.org/html/rfc9110#section-15.5.5",
            Message = message,
            Status = StatusCodes.Status404NotFound,
        };

    public static ObjectResult BadRequestResult(string message)
    {
        return new ObjectResult(Error(message, StatusCodes.Status400BadRequest))
        {
            StatusCode = StatusCodes.Status400BadRequest,
        };
    }

    public static ObjectResult NotFoundResult(string message)
    {
        return new ObjectResult(NotFound(message)) { StatusCode = StatusCodes.Status404NotFound };
    }
    
    public static ErrorResponse Forbidden(string message) =>
        new()
        {
            Type = "https://tools.ietf.org/html/rfc9110#section-15.5.4",
            Message = message,
            Status = StatusCodes.Status403Forbidden,
        };

    public static ObjectResult ForbiddenResult(string message)
    {
        return new ObjectResult(Forbidden(message)) { StatusCode = StatusCodes.Status403Forbidden };
    }

    public static ErrorResponse Conflict(string message) =>
        new()
        {
            Type = "https://tools.ietf.org/html/rfc9110#section-15.5.10",
            Message = message,
            Status = StatusCodes.Status409Conflict,
        };

    public static ObjectResult ConflictResult(string message)
    {
        return new ObjectResult(Conflict(message)) { StatusCode = StatusCodes.Status409Conflict };
    }

    public static ErrorResponse ServerError(string? traceId = null) =>
        new()
        {
            Type = "https://tools.ietf.org/html/rfc9110#section-15.5.22",
            Message = "We ran into an unexpected error. Please try again.",
            Status = StatusCodes.Status500InternalServerError,
            TraceId = traceId,
        };
}
