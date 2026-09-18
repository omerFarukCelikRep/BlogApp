using System.Reflection;
using BlogApp.Core.Email.Abstractions;

namespace BlogApp.Infrastructure.Email.Services;

public class EmailTemplateService : IEmailTemplateService
{
    private const string ResourceName = "BlogApp.Infrastructure.Email.Templates";

    private static string ReadTemplate(string resourceName)
    {
        var assembly = Assembly.GetExecutingAssembly();
        using var stream = assembly.GetManifestResourceStream(resourceName);

        if (stream is null)
            throw new InvalidOperationException($"Email template resource not found: {resourceName}");

        using var reader = new StreamReader(stream);
        return reader.ReadToEnd();
    }

    public string Build(Dictionary<string, string> args, string template)
    {
        var html = ReadTemplate($"{ResourceName}.{template}Email.html");

        html = args.Aggregate(html, (current, value) => current.Replace($"{{{value.Key}}}", value.Value));
        return html;
    }
}