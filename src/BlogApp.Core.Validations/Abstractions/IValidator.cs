using BlogApp.Core.Validations.Results;

namespace BlogApp.Core.Validations.Abstractions;

public interface IValidator<in T>
{
    Task<ValidationResult> ValidateAsync(T arg, CancellationToken cancellationToken = default);
}