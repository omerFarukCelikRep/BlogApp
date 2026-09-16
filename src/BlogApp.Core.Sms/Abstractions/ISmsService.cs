namespace BlogApp.Core.Sms.Abstractions;

public interface ISmsService
{
    Task SendAsync(string to, string message, CancellationToken cancellationToken = default);
}