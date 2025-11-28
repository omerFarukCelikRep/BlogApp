using System.Reflection.Metadata;
using BlogApp.Core.Mediator.Abstractions;
using BlogApp.Core.Mediator.ValueObjects;
using Microsoft.Extensions.DependencyInjection;

namespace BlogApp.Core.Mediator.Handlers;

public class Mediator(IServiceProvider serviceProvider)
    : IMediator
{
    private IRequestHandler<TRequest>? ResolveHandlerFromProvider<TRequest>()
        where TRequest : IRequest
    {
        return serviceProvider.GetService<IRequestHandler<TRequest>>();
    }

    private IRequestHandler<TRequest, TResponse>? ResolveHandlerFromProvider<TRequest, TResponse>()
        where TRequest : IRequest<TResponse>
    {
        return serviceProvider.GetRequiredService<IRequestHandler<TRequest, TResponse>>();
    }

    public Task Send<TRequest>(TRequest request, CancellationToken cancellationToken = default)
        where TRequest : IRequest
    {
        ArgumentNullException.ThrowIfNull(request);

        var handler = ResolveHandlerFromProvider<TRequest>()
                      ?? throw new InvalidOperationException($"No handler registered for {typeof(TRequest).Name}");

        return serviceProvider.GetServices<IPipelineBehavior<TRequest, NonResponse>>()
            .Reverse()
            .Aggregate((RequestHandlerDelegate<NonResponse>)Handler,
                (next, pipeline) => ct => pipeline.Handle(request, next, ct == default ? cancellationToken : ct))(
                cancellationToken);

        async Task<NonResponse> Handler(CancellationToken ct = default)
        {
            await handler.Handle(request, ct == default ? cancellationToken : ct);
            return NonResponse.Value;
        }
    }

    public Task<TResponse> Send<TRequest, TResponse>(TRequest request,
        CancellationToken cancellationToken = default) where TRequest : IRequest<TResponse>
    {
        var handler = ResolveHandlerFromProvider<TRequest, TResponse>()
                      ?? throw new InvalidOperationException(
                          $"No handler registered for {typeof(IRequest<TResponse>).Name}");

        return serviceProvider.GetServices<IPipelineBehavior<TRequest, TResponse>>()
            .Reverse()
            .Aggregate((RequestHandlerDelegate<TResponse>)Handler,
                (next, pipeline) => ct => pipeline.Handle(request, next, ct == default ? cancellationToken : ct))(
                cancellationToken);

        Task<TResponse> Handler(CancellationToken ct = default) =>
            handler.Handle(request, ct == default ? cancellationToken : ct);
    }
}