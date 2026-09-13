using System.Text.Json;
using Microsoft.Extensions.Localization;

namespace BlogApp.Api.Localization;

public partial class JsonStringLocalizer(string resourcePath, ILogger logger) : IStringLocalizer
{
    private readonly Dictionary<string, string> _resources = [];
    private bool _loaded;

    public LocalizedString this[string name]
    {
        get
        {
            EnsureLoaded();
            var value = _resources.TryGetValue(name, out var localizedString) ? localizedString : name;
            return new LocalizedString(name, value, !_resources.ContainsKey(name));
        }
    }

    public LocalizedString this[string name, params object[] arguments]
    {
        get
        {
            EnsureLoaded();
            var localized = this[name];
            return !localized.ResourceNotFound
                ? new LocalizedString(name, string.Format(localized.Value, arguments), false)
                : localized;
        }
    }

    private void EnsureLoaded()
    {
        if (_loaded)
            return;
        
        if (!File.Exists(resourcePath))
        {
            LogJsonResourceFileNotFound(logger, resourcePath);
            return;
        }

        try
        {
            var json = File.ReadAllText(resourcePath);
            var data = JsonSerializer.Deserialize<Dictionary<string, string>>(json);
            if (data == null)
                return;

            foreach (var (key, value) in data)
                _resources[key] = value;

            _loaded = true;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to parse JSON resource file: {File}", resourcePath);
        }
    }

    public IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures)
    {
        EnsureLoaded();
        return _resources.Select(kvp => new LocalizedString(kvp.Key, kvp.Value, false));
    }


    [LoggerMessage(LogLevel.Error, "JSON resource file not found: {filePath}")]
    static partial void LogJsonResourceFileNotFound(ILogger logger, string filePath);
}