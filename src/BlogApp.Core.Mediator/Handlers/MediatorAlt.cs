using BlogApp.Core.Mediator.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace BlogApp.Core.Mediator.Handlers;

public class MediatorAlt(IServiceProvider serviceProvider) : IMediator
{
    public async Task Send<TRequest>(TRequest request, CancellationToken cancellationToken = default)
        where TRequest : IRequest
    {
        var requestType = request.GetType();
        var responseType = typeof(NonResponse);
        var handlerType = typeof(IRequestHandler<>).MakeGenericType(requestType);

        var handler = serviceProvider.GetRequiredService(handlerType);

        var pipelineType = typeof(IPipelineBehavior<,>).MakeGenericType(requestType, responseType);
        var pipelines = serviceProvider.GetServices(pipelineType).Reverse().ToList();

        RequestHandlerDelegate handlerDelegate = () =>
        {
            var method = handlerType.GetMethod(nameof(IRequestHandler<TRequest>.Handle));
            return (Task)method!.Invoke(handler, [request, cancellationToken])!;
        };

        foreach (var pipeline in pipelines)
        {
            var next = handlerDelegate;
            handlerDelegate = () =>
            {
                var method = pipelineType.GetMethod(nameof(IPipelineBehavior<TRequest, NonResponse>.Handle));
                return (Task)method!.Invoke(pipeline, [request, next, cancellationToken])!;
            };
        }

        await handlerDelegate();
    }

    public async Task<TResponse> Send<TResponse>(IRequest<TResponse> request,
        CancellationToken cancellationToken = default)
    {
        var requestType = request.GetType();
        var responseType = typeof(TResponse);
        var handlerType = typeof(IRequestHandler<,>).MakeGenericType(requestType, responseType);

        var handler = serviceProvider.GetRequiredService(handlerType);

        var pipelineType = typeof(IPipelineBehavior<,>).MakeGenericType(requestType, responseType);
        var pipelines = serviceProvider.GetServices(pipelineType).Reverse().ToList();

        RequestHandlerDelegate<TResponse> handlerDelegate = () =>
        {
            var method = handlerType.GetMethod(nameof(IRequestHandler<IRequest<TResponse>, TResponse>.Handle));
            return (Task<TResponse>)method!.Invoke(handler, [request, cancellationToken])!;
        };

        foreach (var pipeline in pipelines)
        {
            var next = handlerDelegate;
            handlerDelegate = () =>
            {
                var method = pipelineType.GetMethod(nameof(IPipelineBehavior<IRequest<TResponse>, TResponse>.Handle));
                return (Task<TResponse>)method!.Invoke(pipeline, [request, next, cancellationToken])!;
            };
        }

        return await handlerDelegate();
    }
}