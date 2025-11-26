using BlogApp.Application.SingingKeys.Commands;
using BlogApp.Core.Mediator.Abstractions;
using BlogApp.Domain.Options;
using Microsoft.Extensions.Options;

namespace BlogApp.Api.BackgroundServices;

public class KeyRotationBackgroundService(IServiceProvider serviceProvider) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            await using var scope = serviceProvider.CreateAsyncScope();
            var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
            var options = scope.ServiceProvider.GetRequiredService<IOptions<KeyRotationOptions>>().Value;
            await mediator.Send(new RotateKeyCommand(), stoppingToken);
            await Task.Delay(options.Period, stoppingToken);
        }
    }
}