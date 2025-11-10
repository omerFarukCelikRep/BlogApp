using BlogApp.Api.Options;
using BlogApp.Application.SingingKeys.Commands;
using BlogApp.Core.Mediator.Abstractions;
using Microsoft.Extensions.Options;

namespace BlogApp.Api.BackgroundServices;

public class KeyRotationBackgroundService(IOptions<KeyRotationOptions> options, IMediator mediator) : BackgroundService
{
    private readonly TimeSpan _rotationPeriod = options.Value.Period;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            await mediator.Send(new RotateKeyCommand(), stoppingToken);
            await Task.Delay(_rotationPeriod, stoppingToken);
        }
    }
}