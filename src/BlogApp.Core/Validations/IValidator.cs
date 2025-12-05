namespace BlogApp.Core.Validations;

public interface IValidator<in T>
{
    Task<ValidationResult> ValidateAsync(T arg, CancellationToken cancellationToken = default);
}