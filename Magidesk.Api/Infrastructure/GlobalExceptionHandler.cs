using Magidesk.Domain.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.EntityFrameworkCore;

namespace Magidesk.Api.Infrastructure;

public class GlobalExceptionHandler : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger;

    public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
    {
        _logger = logger;
    }

    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        _logger.LogError(exception, "Unhandled exception occurred.");

        var (statusCode, title, detail) = MapException(exception);

        httpContext.Response.StatusCode = statusCode;
        
        // Return ProblemDetails standard response
        await httpContext.Response.WriteAsJsonAsync(new Microsoft.AspNetCore.Mvc.ProblemDetails
        {
            Title = title,
            Detail = detail,
            Status = statusCode,
            Instance = httpContext.Request.Path
        }, cancellationToken);

        return true;
    }

    private static (int StatusCode, string Title, string Detail) MapException(Exception exception)
    {
        return exception switch
        {
            BusinessRuleViolationException bw => (StatusCodes.Status400BadRequest, "Business Rule Violation", bw.Message),
            ArgumentException ae => (StatusCodes.Status400BadRequest, "Invalid Argument", ae.Message),
            KeyNotFoundException knf => (StatusCodes.Status404NotFound, "Resource Not Found", knf.Message),
            UnauthorizedAccessException ua => (StatusCodes.Status403Forbidden, "Unauthorized", ua.Message),
            DbUpdateConcurrencyException => (StatusCodes.Status409Conflict, "Concurrency Conflict", "The resource was modified by another user. Please reload and try again."),
            InvalidOperationException ioe => (StatusCodes.Status409Conflict, "Invalid Operation", ioe.Message), // Often used for state conflicts
            _ => (StatusCodes.Status500InternalServerError, "Server Error", "An internal error occurred.")
        };
    }
}
