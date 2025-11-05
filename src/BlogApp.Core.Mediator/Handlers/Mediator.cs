using BlogApp.Core.Mediator.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace BlogApp.Core.Mediator.Handlers;

public class Mediator(IServiceProvider serviceProvider, Registry registry)
    : IMediator
{
    private void ValidateHandler<TRequest>()
    {
        if (!registry.HasHandler<TRequest>())
            throw new InvalidOperationException($"No handler registered for {typeof(TRequest).Name}");
    }

    private IRequestHandler<TRequest>? ResolveHandlerFromProvider<TRequest>(IRequest request)
        where TRequest : IRequest
    {
        var handlerType = typeof(IRequestHandler<>).MakeGenericType(request.GetType());
        return serviceProvider.GetService(handlerType) as IRequestHandler<TRequest>;
    }

    private IRequestHandler<TRequest, TResponse>? ResolveHandlerFromProvider<TRequest, TResponse>(
        IRequest<TResponse> request)
        where TRequest : IRequest<TResponse>
    {
        var handlerType = typeof(IRequestHandler<,>).MakeGenericType(request.GetType(), typeof(TResponse));
        return serviceProvider.GetService(handlerType) as IRequestHandler<TRequest, TResponse>;
    }

    public async Task Send<TRequest>(TRequest request, CancellationToken cancellationToken = default)
        where TRequest : IRequest
    {
        var handler = registry.GetHandler<TRequest>()
                      ?? ResolveHandlerFromProvider<TRequest>(request);
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

    public async Task<TResponse> Send<TResponse>(IRequest<TResponse> request,
        CancellationToken cancellationToken = default)
    {
        var handler = registry.GetHandler<IRequest<TResponse>, TResponse>()
                      ?? ResolveHandlerFromProvider<IRequest<TResponse>, TResponse>(request);

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