using BlogApp.Core.Caching.Abstractions;
using BlogApp.Core.Caching.Markers;
using BlogApp.Core.Mediator.Abstractions;

namespace BlogApp.Core.Caching.Behaviors;

public class CachingBehavior<TRequest, TResponse>(ICacheService cacheService)
    : IPipelineBehavior<TRequest, TResponse> where TRequest : notnull
{
    private async Task<TResponse> HandleCacheable(ICacheable cacheable, RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken = default)
    {
        var cached = await cacheService.GetAsync<TResponse>(cacheable.Key, cancellationToken);
        if (cached is not null)
            return cached;

        var response = await next(cancellationToken);
        if (response is not null)
            await cacheService.SetAsync(cacheable.Key, response, cacheable.Expiry, cancellationToken);

        return response;
    }

    private async Task<TResponse> HandleInvalidating(ICacheInvalidating invalidating,
        RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken = default)
    {
        var response = await next(cancellationToken);

        await Task.WhenAll(invalidating.InvalidationKeys.Select(key => cacheService.RemoveAsync(key, cancellationToken)));

        return response;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken = default)
    {
        return request switch
        {
            ICacheable cacheableRequest => await HandleCacheable(cacheableRequest, next, cancellationToken),
            ICacheInvalidating invalidatedCacheRequest => await HandleInvalidating(invalidatedCacheRequest, next,
                cancellationToken),
            _ => await next(cancellationToken)
        };
    }
}