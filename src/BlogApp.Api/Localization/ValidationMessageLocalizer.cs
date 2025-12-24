using System.Reflection;
using BlogApp.Api.Resources;
using BlogApp.Core.Validations.Abstractions;
using BlogApp.Core.Validations.Utils;
using Microsoft.Extensions.Localization;

namespace BlogApp.Api.Localization;

public class ValidationMessageLocalizer(IStringLocalizerFactory localizerFactory) : IValidationMessageLocalizer
{
    private readonly IStringLocalizer _localizer = localizerFactory.Create(Constants.ResourceName,
        new AssemblyName(typeof(Shared).Assembly.FullName!).Name!);

    public string Get(string key, string defaultMessage, IReadOnlyDictionary<string, string>? args = null)
    {
        if (string.IsNullOrWhiteSpace(key))
            return defaultMessage;

        var localizedString = _localizer[key];
        if (!localizedString.ResourceNotFound && args is not null)
            return args.Aggregate(localizedString.Value,
                (currentMessage, arg) => currentMessage.Replace($"{{{arg.Key}}}", arg.Value));

        return localizedString.ResourceNotFound
            ? defaultMessage
            : localizedString;
    }
}