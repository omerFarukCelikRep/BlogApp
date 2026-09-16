using BlogApp.Core.Sms.Abstractions;
using BlogApp.Core.Sms.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Twilio;
using Twilio.Rest.Api.V2010.Account;

namespace BlogApp.Infrastructure.Sms.Services;

public class TwilioSmsService(IOptions<SmsOptions> options, ILogger<TwilioSmsService> logger) : ISmsService
{
    private readonly SmsOptions _options = options.Value;

    public async Task SendAsync(string to, string message, CancellationToken cancellationToken = default)
    {
        try
        {
            TwilioClient.Init(_options.AccountSid, _options.AuthToken);

            var msg = await MessageResource.CreateAsync(to: new Twilio.Types.PhoneNumber(to),
                from: new Twilio.Types.PhoneNumber(_options.FromNumber), body: message);

            logger.LogInformation("SMS sent.");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to send SMS.");
            throw;
        }
    }
}