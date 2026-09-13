namespace BlogApp.Core.Email.Models;

public record EmailMessage(
    string To,
    string Subject,
    string HtmlBody,
    bool IsBodyHtml,
    string? PlainTextBody = null);