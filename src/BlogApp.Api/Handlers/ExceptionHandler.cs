using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace BlogApp.Api.Handlers;

public sealed class ExceptionHandler(ILogger<ExceptionHandler> logger, IProblemDetailsService problemDetailsService) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception,
        CancellationToken cancellationToken)
    {
        var exceptionType = exception.GetType().Name;
        var path = httpContext.Request.Path;
        logger.LogError(exception, "{Path} - {Exception} : {Message}. TraceId:{TraceId}", path,exceptionType,exception.Message, httpContext.TraceIdentifier);
        
        var (statusCode, message) = exception switch
        {
            KeyNotFoundException => (StatusCodes.Status404NotFound, "Resource Not Found"),
            ArgumentException => (StatusCodes.Status400BadRequest, "Invalid Request"),
            _ => (StatusCodes.Status500InternalServerError, "Internal Server Error"),
        };
        var problemDetails = new ProblemDetails
        {
            Status = statusCode,
            Title = message,
            Type = exceptionType,
            Instance = path,
            Detail = exception.Message,
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