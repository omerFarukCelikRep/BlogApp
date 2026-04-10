namespace BlogApp.Core.Localization;

public interface IMessageLocalizer
{
    string Get(string key, string? defaultMessage = null, IReadOnlyDictionary<string, string>? args = null);
}