namespace BlogApp.Core.Email.Abstractions;

public interface IEmailTemplateService
{
    string Build(Dictionary<string, string> args, string template);
}