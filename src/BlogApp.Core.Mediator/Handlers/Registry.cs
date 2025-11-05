using BlogApp.Core.Mediator.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace BlogApp.Core.Mediator.Handlers;

public class Registry
{
    private readonly Dictionary<Type, object> _handlers = [];
    private readonly Dictionary<Type, object> _behaviors = [];

    public void AddHandler<TRequest>(IRequestHandler<TRequest> handler)
        where TRequest : IRequest =>
        _handlers[typeof(TRequest)] = handler;

    public void AddHandler<TRequest, TResponse>(IRequestHandler<TRequest, TResponse> handler)
        where TRequest : IRequest<TResponse> =>
        _handlers[typeof(TRequest)] = handler;

    public void AddBehavior<TRequest, TResponse, TBehavior>()
        where TRequest : IRequest<TResponse>
        where TBehavior : IPipelineBehavior<TRequest, TResponse>
    {
        var key = typeof(TRequest);
        if (!_behaviors.TryGetValue(key, out var list))
        {
            list = new List<Type>();
            _behaviors[key] = list;
        }

        var typeList = (List<Type>)list;
        if (!typeList.Contains(typeof(TBehavior)))
            typeList.Add(typeof(TBehavior));
    }

    public bool HasHandler<TRequest>() => _handlers.ContainsKey(typeof(TRequest));

    public IRequestHandler<TRequest>? GetHandler<TRequest>() where TRequest : IRequest
    {
        _handlers.TryGetValue(typeof(TRequest), out var value);
        return value as IRequestHandler<TRequest>;
    }

    public IRequestHandler<TRequest, TResponse>? GetHandler<TRequest, TResponse>() where TRequest : IRequest<TResponse>
    {
        _handlers.TryGetValue(typeof(TRequest), out var value);
        return value as IRequestHandler<TRequest, TResponse>;
    }

    public IEnumerable<IPipelineBehavior<TRequest, TResponse>> ResolveBehaviors<TRequest, TResponse>(
        IServiceProvider provider)
        where TRequest : IRequest<TResponse>
    {
        if (!_behaviors.TryGetValue(typeof(TRequest), out var types))
            yield break;

        var behaviors = types as List<Type> ?? [];
        foreach (var instance in behaviors.Select(provider.GetService))
        {
            if (instance is IPipelineBehavior<TRequest, TResponse> behavior)
                yield return behavior;
        }
    }
}