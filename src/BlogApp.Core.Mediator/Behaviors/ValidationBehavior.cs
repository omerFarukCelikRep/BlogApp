using BlogApp.Core.Exceptions;
using BlogApp.Core.Mediator.Abstractions;
using BlogApp.Core.Validations;
using Microsoft.Extensions.Logging;

namespace BlogApp.Core.Mediator.Behaviors;

public class ValidationBehavior<TRequest, TResponse>(
    IEnumerable<Validations.IValidator<TRequest>> validators,
    ILogger<ValidationBehavior<TRequest, TResponse>> logger)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken = default)
    {
        if (!validators.Any())
            return await next(cancellationToken);

        List<ValidationError> errors = [];
        foreach (var validator in validators)
        {
            var validationResult = await validator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
                errors.AddRange(validationResult.Errors);
        }

        if (errors is not { Count: > 0 })
            return await next(cancellationToken);

        logger.LogError("Validation failed for {RequestName}: {Errors}", typeof(TRequest).FullName,
            string.Join(", ", errors));

        throw new ValidationException(errors);
    }
}