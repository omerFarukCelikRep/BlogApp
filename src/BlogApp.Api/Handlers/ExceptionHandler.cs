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
            KeyNotFoundException => (StatusCodes.Status404NotFound, "Resource Not Found"),
            ArgumentException => (StatusCodes.Status400BadRequest, "Invalid Request"),
            ValidationException validationException => (StatusCodes.Status400BadRequest,
                string.Join(',',
                    validationException.PropertyExceptions.Select(x => $"{x.PropertyName} : {x.ErrorMessage}"))),
            _ => (StatusCodes.Status500InternalServerError, "Internal Server Error"),
        };
        var problemDetails = new ProblemDetails
        {
            Status = statusCode,
            Title = exception.Message,
            Type = exceptionType,
            Instance = path,
            Detail = message,
        };

        problemDetails.Extensions["traceId"] = httpContext.TraceIdentifier;
        problemDetails.Extensions["timeStamp"] = DateTimeOffset.UtcNow;

        await problemDetailsService.WriteAsync(new ProblemDetailsContext
        {
            HttpContext = httpContext,
            Exception = exception,
            ProblemDetails = problemDetails
        });

        return true;
    }
}