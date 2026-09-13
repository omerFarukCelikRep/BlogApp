using System.Reflection;
using BlogApp.Api.Resources;
using BlogApp.Core.Validations.Abstractions;
using BlogApp.Core.Validations.Utils;
using BlogApp.Domain.Constants;
using Microsoft.Extensions.Localization;

namespace BlogApp.Api.Localization;

public class ValidationMessageLocalizer(IStringLocalizerFactory localizerFactory) : IValidationMessageLocalizer
{
    private readonly IStringLocalizer _localizer = localizerFactory.Create("Validations", string.Empty);

    public string Localize(string errorCode, string? defaultMessage = null,
        IReadOnlyDictionary<string, string>? args = null)
    {
        if (string.IsNullOrWhiteSpace(errorCode))
            return defaultMessage ?? Errors.MessageNotFound;

        var localizedString = _localizer[errorCode];
        if (!localizedString.ResourceNotFound && args is not null)
            return args.Aggregate(localizedString.Value,
                (currentMessage, arg) => currentMessage.Replace($"{{{arg.Key}}}", arg.Value));

        return localizedString.ResourceNotFound
            ? defaultMessage ?? Errors.MessageNotFound
            : localizedString;
    }
}