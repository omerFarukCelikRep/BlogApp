using System.Net;
using System.Net.Mail;
using BlogApp.Core.Email.Abstractions;
using BlogApp.Core.Email.Models;
using BlogApp.Core.Email.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace BlogApp.Infrastructure.Email.Services;

public class SmtpEmailService(IOptions<EmailOptions> options, ILogger<SmtpEmailService> logger) : IEmailService
{
    private readonly EmailOptions _emailOptions = options.Value;

    public async Task SendAsync(EmailMessage message, CancellationToken cancellationToken = default)
    {
        try
        {
            using var client = new SmtpClient(_emailOptions.Host, _emailOptions.Port)
            {
                EnableSsl = _emailOptions.EnableSsl,
                Credentials = new NetworkCredential(_emailOptions.Username, _emailOptions.Password)
            };

            using var mail = new MailMessage
            {
                From = new MailAddress(_emailOptions.FromAddress, _emailOptions.FromName),
                Subject = message.Subject,
                Body = message.HtmlBody,
                IsBodyHtml = message.IsBodyHtml
            };

            mail.To.Add(message.To);
            if (message.PlainTextBody is not null)
                mail.AlternateViews.Add(
                    AlternateView.CreateAlternateViewFromString(message.PlainTextBody, null, "text/plain"));

            await client.SendMailAsync(mail, cancellationToken);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to send email {Subject}", message.Subject);
            throw;
        }
    }
}