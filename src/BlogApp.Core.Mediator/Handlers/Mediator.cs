using BlogApp.Core.Mediator.Abstractions;
using BlogApp.Core.Mediator.ValueObjects;
using Microsoft.Extensions.DependencyInjection;

namespace BlogApp.Core.Mediator.Handlers;

public class Mediator(IServiceProvider serviceProvider, Registry registry)
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

    public async Task Send<TRequest>(TRequest request, CancellationToken cancellationToken = default)
        where TRequest : IRequest
    {
        var handler = ResolveHandlerFromProvider<TRequest>();
        if (handler is null)
            throw new InvalidOperationException($"No handler registered for {typeof(TRequest).Name}");

        var behaviors = registry.ResolveBehaviors<IRequest<NonResponse>, NonResponse>(serviceProvider)
            .Reverse()
            .ToList();

        RequestHandlerDelegate<NonResponse> handlerDelegate = async () =>
        {
            await handler.Handle(request, cancellationToken);
            return NonResponse.Value;
        };

        foreach (var pipelineBehavior in behaviors)
        {
            var next = handlerDelegate;
            handlerDelegate = () => pipelineBehavior.Handle((dynamic)request, next, cancellationToken);
        }

        await handlerDelegate();
    }

    public async Task<TResponse> Send<TRequest, TResponse>(TRequest request,
        CancellationToken cancellationToken = default) where TRequest : IRequest<TResponse>
    {
        var handler = ResolveHandlerFromProvider<TRequest, TResponse>();

        if (handler is null)
            throw new InvalidOperationException($"No handler registered for {typeof(IRequest<TResponse>).Name}");

        var behaviors = registry.ResolveBehaviors<IRequest<TResponse>, TResponse>(serviceProvider)
            .Reverse()
            .ToList();

        RequestHandlerDelegate<TResponse> handlerDelegate = () => handler.Handle(request, cancellationToken);

        foreach (var pipelineBehavior in behaviors)
        {
            var next = handlerDelegate;
            handlerDelegate = () => pipelineBehavior.Handle(request, next, cancellationToken);
        }

        return await handlerDelegate();
    }
}