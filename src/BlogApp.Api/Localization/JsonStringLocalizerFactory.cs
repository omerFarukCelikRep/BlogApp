using BlogApp.Api.Resources;
using Microsoft.Extensions.Localization;

namespace BlogApp.Api.Localization;

public class JsonStringLocalizerFactory(ILogger logger) : IStringLocalizerFactory
{
    public IStringLocalizer Create(Type resourceSource)
    {
        return Create(resourceSource.Name, "");
    }

    public IStringLocalizer Create(string baseName, string location)
    {
        var resourceName = nameof(Shared);
        if (!string.IsNullOrWhiteSpace(baseName))
            resourceName = baseName;

        var cultureName = Thread.CurrentThread.CurrentUICulture.Name;

        return new JsonStringLocalizer(resourceName, cultureName, logger);
    }
}