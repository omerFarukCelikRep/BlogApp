using System.Reflection;
using BlogApp.Core.Security.Abstractions;
using BlogApp.Core.Security.Attributes;
using BlogApp.Core.Security.Options;
using Microsoft.Extensions.Options;

namespace BlogApp.Api.Filters;

public sealed class XssProtectionEndpointFilter(IXssSanitizer xssSanitizer, IOptions<XssOptions> options)
    : IEndpointFilter
{
    private readonly XssOptions _options = options.Value;

    private void SanitizeObject(object obj, Type type)
    {
        var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(x => x.CanWrite && x.PropertyType == typeof(string));

        foreach (var property in properties)
        {
            var value = property.GetValue(obj) as string;
            if (string.IsNullOrEmpty(value))
                continue;

            var sanitizeAttribute = property.GetCustomAttribute<SanitizeAttribute>();

            string sanitized;
            if (sanitizeAttribute is not null)
                sanitized = xssSanitizer.Sanitize(value, sanitizeAttribute);
            else if (_options.StripHtmlByDefault)
                sanitized = xssSanitizer.StripHtml(value);
            else
                continue;

            if (sanitized != value)
                property.SetValue(obj, sanitized);
        }
    }

    private object SanitizeArgument(object arg)
    {
        var type = arg.GetType();
        if (type.IsPrimitive || type.IsEnum)
            return arg;

        if (arg is string str)
            return _options.StripHtmlByDefault
                ? xssSanitizer.StripHtml(str)
                : str;

        SanitizeObject(arg, type);

        return arg;
    }

    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        for (var i = 0; i < context.Arguments.Count; i++)
        {
            var argument = context.Arguments[i];
            if (argument is null)
                continue;

            context.Arguments[i] = SanitizeArgument(argument);
        }

        return await next(context);
    }
}