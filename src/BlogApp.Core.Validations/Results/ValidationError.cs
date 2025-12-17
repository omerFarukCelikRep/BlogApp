namespace BlogApp.Core.Validations.Results;

public sealed record ValidationError(string PropertyName, string ErrorMessage);