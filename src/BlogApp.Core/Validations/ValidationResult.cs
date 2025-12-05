namespace BlogApp.Core.Validations;

public class ValidationResult
{
    public bool IsValid => Errors.Count == 0;
    public List<ValidationError> Errors { get; init; } = [];

    public static ValidationResult Success => new();
    public static ValidationResult Failure(params List<ValidationError> errors) => new() { Errors = errors };
    public void AddError(string propertyName, string errorMessage) => Errors.Add(new(propertyName, errorMessage));
}