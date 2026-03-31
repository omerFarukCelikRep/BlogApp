using System.Text.Json;
using BlogApp.Core.Caching.Abstractions;
using BlogApp.Core.Caching.Options;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;

namespace BlogApp.Infrastructure.Caching.Services;

public class RedisCacheService(IDistributedCache cache, IOptions<CacheOptions> options) : ICacheService
{
    private readonly CacheOptions _options = options.Value;

    private static readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    public async Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default)
    {
        var json = await cache.GetStringAsync(key, cancellationToken);
        return string.IsNullOrEmpty(json) ? default : JsonSerializer.Deserialize<T?>(json, _jsonOptions);
    }

    public async Task SetAsync<T>(string key, T value, TimeSpan? absoluteExpiration = null,
        CancellationToken cancellationToken = default)
    {
        var serializedData = JsonSerializer.Serialize(value, _jsonOptions);
        await cache.SetStringAsync(key, serializedData, new DistributedCacheEntryOptions()
        {
            AbsoluteExpirationRelativeToNow = absoluteExpiration ?? _options.Expiration
        }, cancellationToken);
    }

    public async Task RemoveAsync(string key, CancellationToken cancellationToken = default)
    {
        await cache.RemoveAsync(key, cancellationToken);
    }

    public async Task<bool> ExistsAsync(string key, CancellationToken cancellationToken = default)
    {
        var json = await cache.GetStringAsync(key, cancellationToken);
        return !string.IsNullOrEmpty(json);
    }
}