using BlogApp.Core.Email.Models;

namespace BlogApp.Core.Email.Abstractions;

public interface IEmailService
{
    Task SendAsync(EmailMessage message, CancellationToken cancellationToken = default);
}