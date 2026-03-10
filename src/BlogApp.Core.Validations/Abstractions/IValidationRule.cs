using BlogApp.Core.Validations.Results;

namespace BlogApp.Core.Validations.Abstractions;

internal interface IValidationRule<in T>
{
    Task<IEnumerable<ValidationError>> ValidateAsync(T instance, CancellationToken cancellationToken = default);
}