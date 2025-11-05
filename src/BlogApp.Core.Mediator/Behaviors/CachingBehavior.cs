using System.Reflection;
using BlogApp.Core.Caching.Abstractions;
using BlogApp.Core.Caching.Attributes;
using BlogApp.Core.Mediator.Abstractions;

namespace BlogApp.Core.Mediator.Behaviors;

public class CachingBehavior<TRequest, TResponse>(ICacheService cacheService)
    : IPipelineBehavior<TRequest, TResponse> where TRequest : notnull
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken = default)
    {
        var cacheAttribute = request.GetType().GetCustomAttribute<CacheableAttribute>();
        if (cacheAttribute is null)
            return await next();

        var key = cacheAttribute.Key ?? $"{typeof(TRequest).FullName}";

        var cachedResponse = await cacheService.GetAsync<TResponse>(key, cancellationToken);
        if (cachedResponse is not null)
            return cachedResponse;

        var response = await next();

        await cacheService.SetAsync(key, response, TimeSpan.FromSeconds(cacheAttribute.DurationSeconds),
            cancellationToken);

        return response;
    }
}