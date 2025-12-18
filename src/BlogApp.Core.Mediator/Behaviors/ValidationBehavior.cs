using BlogApp.Core.Mediator.Abstractions;
using BlogApp.Core.Validations.Abstractions;
using BlogApp.Core.Validations.Exceptions;
using BlogApp.Core.Validations.Results;
using Microsoft.Extensions.Logging;

namespace BlogApp.Core.Mediator.Behaviors;

public partial class ValidationBehavior<TRequest, TResponse>(
    IEnumerable<IValidator<TRequest>> validators,
    IValidationMessageLocalizer localizer,
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

        var localized = errors.Select(x => x with
        {
            ErrorMessage = localizer.Get(x.ErrorCode ?? string.Empty, x.ErrorMessage, x.Args)
        }).ToList();
        LogValidationErrors(logger, typeof(TRequest).FullName!, string.Join(", ", localized));
        throw new ValidationException(localized);
    }

    [LoggerMessage(LogLevel.Error, "Validation failed for {requestName}: {errors}")]
    static partial void LogValidationErrors(ILogger<ValidationBehavior<TRequest, TResponse>> logger, string requestName, string errors);
}