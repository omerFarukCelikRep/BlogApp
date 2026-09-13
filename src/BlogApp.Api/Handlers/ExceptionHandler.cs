using BlogApp.Core.Logging.Contexts;
using BlogApp.Core.Results;
using BlogApp.Core.Security.Exceptions;
using BlogApp.Core.Validations.Exceptions;
using BlogApp.Core.Validations.Results;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace BlogApp.Api.Handlers;

public sealed class ExceptionHandler(ILogger<ExceptionHandler> logger, IProblemDetailsService problemDetailsService)
    : IExceptionHandler
{
    private static Result<List<ValidationError>> BuildValidationResult(ValidationException ve)
    {
        return Result<List<ValidationError>>.Failed(400,
            Error.Create("VALIDATION_ERROR", "One or more validation errors occurred."), ve.PropertyExceptions);
    }

    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception,
        CancellationToken cancellationToken)
    {
        var exceptionType = exception.GetType().Name;
        var path = httpContext.Request.Path;
        logger.LogError(exception, "{Path} - {Exception} : {Message}. TraceId:{TraceId}", path, exceptionType,
            exception.Message, httpContext.TraceIdentifier);

        if (exception is ValidationException validationException)
        {
            var result = BuildValidationResult(validationException);
            await httpContext.Response.WriteAsJsonAsync(result, cancellationToken);
            return true;
        }

        var (statusCode, message) = exception switch
        {
            UnauthorizedAccessException => (StatusCodes.Status401Unauthorized, "Unauthorized"),
            ForbiddenAccessException => (StatusCodes.Status403Forbidden, "Access Denied"),
            KeyNotFoundException => (StatusCodes.Status404NotFound, "Resource Not Found"),
            ArgumentException => (StatusCodes.Status400BadRequest, "Invalid Request"),
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