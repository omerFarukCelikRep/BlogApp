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
    
    private static readonly Dictionary<string, string> _resourceMap = new()
    {
        { "Auth",    "Errors.Auth"    },
        { "Blog",    "Errors.Blog"    },
        { "Comment", "Errors.Comment" },
    };

    public string Localize(string errorCode, string? defaultMessage = null, IReadOnlyDictionary<string, string>? args = null)
    {
        if (string.IsNullOrWhiteSpace(errorCode))
            return Errors.MessageNotFound;
        
        var parts = errorCode.Split('.');
        var prefix = parts.Length > 1 ? parts[0] : string.Empty;
        if (!_resourceMap.TryGetValue(prefix, out var resourceName))
            return errorCode;

        var localizer = localizerFactory.Create(resourceName, string.Empty);
        var localized = localizer[errorCode];
        
        return localized.ResourceNotFound
            ? defaultMessage ?? errorCode
            : localized.Value;
    }
}