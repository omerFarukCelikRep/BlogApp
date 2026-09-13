namespace BlogApp.Core.Localization;

public interface IMessageLocalizer
{
    string Localize(string errorCode, string? defaultMessage = null, IReadOnlyDictionary<string, string>? args = null);
}