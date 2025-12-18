namespace BlogApp.Core.Validations.Abstractions;

public interface IValidationMessageLocalizer
{
    string Get(string key, string defaultMessage, IReadOnlyDictionary<string, string>? args = null);
}