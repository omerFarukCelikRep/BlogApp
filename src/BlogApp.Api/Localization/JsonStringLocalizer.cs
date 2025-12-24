using System.Text.Json;
using Microsoft.Extensions.Localization;

namespace BlogApp.Api.Localization;

public partial class JsonStringLocalizer : IStringLocalizer
{
    private readonly string _resourceName;
    private readonly string _cultureName;
    private readonly Dictionary<string, string> _localizedStrings = [];

    public LocalizedString this[string name]
    {
        get
        {
            var value = _localizedStrings.TryGetValue(name, out var localizedString) ? localizedString : name;
            return new LocalizedString(name, value, !_localizedStrings.ContainsKey(name));
        }
    }

    public LocalizedString this[string name, params object[] arguments]
    {
        get
        {
            var localized = this[name];
            return !localized.ResourceNotFound
                ? new LocalizedString(name, string.Format(localized.Value, arguments), false)
                : localized;
        }
    }

    public JsonStringLocalizer(string resourceName, string cultureName, ILogger logger)
    {
        _resourceName = resourceName;
        _cultureName = cultureName;
        LoadJson(logger);
    }

    private void LoadJson(ILogger logger)
    {
        var filePath = Path.Combine("Resources", _cultureName, $"{_resourceName}.json");
        if (!File.Exists(filePath))
        {
            LogJsonResourceFileNotFound(logger, filePath);
            return;
        }

        try
        {
            var json = File.ReadAllText(filePath);
            var data = JsonSerializer.Deserialize<Dictionary<string, string>>(json);
            if (data == null)
                return;

            foreach (var (key, value) in data)
                _localizedStrings[key] = value;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to parse JSON resource file: {FilePath}", filePath);
        }
    }

    public IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures)
    {
        return _localizedStrings.Select(kvp => new LocalizedString(kvp.Key, kvp.Value, false));
    }


    [LoggerMessage(LogLevel.Error, "JSON resource file not found: {filePath}")]
    static partial void LogJsonResourceFileNotFound(ILogger logger, string filePath);
}