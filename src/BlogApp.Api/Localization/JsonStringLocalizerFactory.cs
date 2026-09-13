using Microsoft.Extensions.Localization;

namespace BlogApp.Api.Localization;

public class JsonStringLocalizerFactory(ILogger logger) : IStringLocalizerFactory
{
    private readonly string _resourcesPath = Path.Combine(AppContext.BaseDirectory, "Resources");
    private readonly Dictionary<string, IStringLocalizer> _cache = [];

    public IStringLocalizer Create(Type resourceSource)
    {
        return Create(resourceSource.Name, string.Empty);
    }

    private string BuildPath(string baseName, string culture) => Path.Combine(_resourcesPath,culture,$"{baseName}.json");

    private string ResolveResourceFile(string baseName, string culture)
    {
        var exact = BuildPath(baseName, culture);
        if (File.Exists(exact))
            return exact;

        var neutral = culture.Contains('-')
            ? BuildPath(baseName, culture.Split('-').First())
            : null;
        if (neutral is not null && File.Exists(neutral))
            return neutral;

        return BuildPath(baseName, "tr-TR");
    }

    public IStringLocalizer Create(string baseName, string location)
    {
        var cultureName = Thread.CurrentThread.CurrentUICulture.Name;
        var cacheKey = $"{baseName}:{cultureName}";
        if (_cache.TryGetValue(cacheKey, out var localizer))
            return localizer;

        var resourceFile = ResolveResourceFile(baseName, cultureName);
        localizer = new JsonStringLocalizer(resourceFile, logger);

        _cache[cacheKey] = localizer;
        return localizer;
    }
}