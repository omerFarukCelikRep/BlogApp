using System.Reflection;
using BlogApp.Core.Email.Abstractions;

namespace BlogApp.Infrastructure.Email.Services;

public class EmailTemplateService :IEmailTemplateService 
{
    private const string ResourceName = "BlogApp.Infrastructure.Email.Templates.ConfirmationEmail.html";

    private static string ReadTemplate()
    {
        var assembly = Assembly.GetExecutingAssembly();
        using var stream = assembly.GetManifestResourceStream(ResourceName);

        if (stream is null)
            throw new InvalidOperationException($"Email template resource not found: {ResourceName}");

        using var reader = new StreamReader(stream);
        return reader.ReadToEnd();
    }

    public string Build(string firstName, string confirmUrl, string otpCode, int expiryMinutes = 30)
    {
        var html = ReadTemplate();

        return html
            .Replace("{{FIRST_NAME}}", firstName)
            .Replace("{{CONFIRM_URL}}", confirmUrl)
            .Replace("{{OTP_CODE}}", otpCode)
            .Replace("{{EXPIRY_MINUTES}}", expiryMinutes.ToString());
    }
}