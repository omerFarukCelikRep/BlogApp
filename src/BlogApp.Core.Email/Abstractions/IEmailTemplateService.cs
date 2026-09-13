namespace BlogApp.Core.Email.Abstractions;

public interface IEmailTemplateService
{
    string Build(string firstName, string confirmUrl, string otpCode, int expiryMinutes = 30);
}