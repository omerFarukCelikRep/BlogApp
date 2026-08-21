using BlogApp.Core.Logging.Contexts;
using BlogApp.Core.Security.Exceptions;
using BlogApp.Core.Validations.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace BlogApp.Api.Handlers;

public sealed class ExceptionHandler(ILogger<ExceptionHandler> logger, IProblemDetailsService problemDetailsService)
    : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception,
        CancellationToken cancellationToken)
    {
        var exceptionType = exception.GetType().Name;
        var path = httpContext.Request.Path;
        logger.LogError(exception, "{Path} - {Exception} : {Message}. TraceId:{TraceId}", path, exceptionType,
            exception.Message, httpContext.TraceIdentifier);

        var (statusCode, message) = exception switch
        {
            UnauthorizedAccessException => (StatusCodes.Status401Unauthorized, "Unauthorized"),
            ForbiddenAccessException => (StatusCodes.Status403Forbidden, "Access Denied"),
            KeyNotFoundException => (StatusCodes.Status404NotFound, "Resource Not Found"),
            ArgumentException => (StatusCodes.Status400BadRequest, "Validation Failed"),
            ValidationException => (StatusCodes.Status400BadRequest, "Invalid Request"),
            _ => (StatusCodes.Status500InternalServerError, "Internal Server Error"),
        };
        var problemDetails = new ProblemDetails
        {
            Status = statusCode,
            Title = exception.Message,
            Type = exceptionType,
            Instance = path,
            Detail = message,
            Extensions =
            {
                ["traceId"] = httpContext.TraceIdentifier,
                ["correlationId"] = CorrelationContext.CurrentId,
                ["timeStamp"] = DateTime.UtcNow
            }
        };

        if (exception is ValidationException validationException)
            problemDetails.Extensions["errors"] =
                validationException.PropertyExceptions.Select(x => new { x.PropertyName, x.ErrorMessage });

        httpContext.Response.StatusCode = statusCode;
        await problemDetailsService.WriteAsync(new()
        {
            HttpContext = httpContext,
            Exception = exception,
            ProblemDetails = problemDetails
        });

        return true;
    }
}