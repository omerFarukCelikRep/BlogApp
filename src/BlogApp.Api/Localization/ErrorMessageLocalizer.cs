using System.Reflection;
using BlogApp.Api.Resources;
using BlogApp.Core.Localization;
using BlogApp.Domain.Constants;
using Microsoft.Extensions.Localization;

namespace BlogApp.Api.Localization;

public class ErrorMessageLocalizer(IStringLocalizerFactory localizerFactory) : IErrorMessageLocalizer
{
    private readonly IStringLocalizer _localizer = localizerFactory.Create(Errors.ResourceName,
        new AssemblyName(typeof(Shared).Assembly.FullName!).Name!);

    public string Get(string key, string? defaultMessage = null, IReadOnlyDictionary<string, string>? args = null)
    {
        if (string.IsNullOrWhiteSpace(key))
            return Errors.MessageNotFound;

        var localizedString = _localizer[key];
        if (!localizedString.ResourceNotFound && args is not null)
            return args.Aggregate(localizedString.Value,
                (currentMessage, arg) => currentMessage.Replace($"{{{arg.Key}}}", arg.Value));

        return localizedString.ResourceNotFound
            ? defaultMessage ?? Errors.MessageNotFound
            : localizedString;
    }
}