using BlogApp.Core.Exceptions;
using BlogApp.Core.Mediator.Abstractions;
using Microsoft.Extensions.Logging;

namespace BlogApp.Core.Mediator.Behaviors;

public class ValidationBehavior<TRequest, TResponse>(
    IEnumerable<IValidator<TRequest>> validators,
    ILogger<ValidationBehavior<TRequest, TResponse>> logger)
    : IPipelineBehavior<TRequest, TResponse> where TRequest : notnull
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken = default)
    {
        if (!validators.Any())
            return await next(cancellationToken);

        List<string> errors = [];
        foreach (var validator in validators)
        {
            var validationErrors = await validator.ValidateAsync(request, cancellationToken);
            errors.AddRange(validationErrors);
        }

        if (errors is not { Count: > 0 }) 
            return await next(cancellationToken);
        
        logger.LogWarning("Validation failed for {RequestName}: {Errors}", typeof(TRequest).FullName,
            string.Join(", ", errors));

        throw new ValidationException();

    }
}