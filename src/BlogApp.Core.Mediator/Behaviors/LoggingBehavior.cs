using BlogApp.Core.Mediator.Abstractions;
using Microsoft.Extensions.Logging;

namespace BlogApp.Core.Mediator.Behaviors;

public class LoggingBehavior<TRequest, TResponse>(ILogger<LoggingBehavior<TRequest, TResponse>> logger)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Handling {Name}", typeof(TRequest).FullName);
        var response = await next();
        logger.LogInformation("Handled {Name}", typeof(TRequest).FullName);
        return response;
    }
}