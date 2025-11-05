namespace BlogApp.Core.Results;

public sealed record Error(string Code, string ErrorMessage)
{
    public static readonly Error None = new(string.Empty, string.Empty);

    public static Error Create(string code, string message) => new(code, message);
}