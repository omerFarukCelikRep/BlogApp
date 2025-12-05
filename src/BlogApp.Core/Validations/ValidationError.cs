namespace BlogApp.Core.Validations;

public sealed record ValidationError(string PropertyName, string ErrorMessage);