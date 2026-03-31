using System.Text.Json;
using BlogApp.Core.Caching.Abstractions;
using BlogApp.Core.Caching.Options;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

namespace BlogApp.Infrastructure.Caching.Services;

public class HybridCacheService(
    IMemoryCache memoryCache,
    IDistributedCache distributedCache,
    IOptions<CacheOptions> options) : ICacheService
{
    private readonly CacheOptions _options = options.Value;

    private static readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    public async Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default)
    {
        if (memoryCache.TryGetValue(key, out T? cached))
            return cached;

        var json = await distributedCache.GetStringAsync(key, cancellationToken);
        if (string.IsNullOrEmpty(json))
            return default;

        var value = JsonSerializer.Deserialize<T>(json, _jsonOptions);
        if (value is not null)
            memoryCache.Set(key, value, new MemoryCacheEntryOptions()
            {
                AbsoluteExpirationRelativeToNow = _options.Expiration / 2,
            });

        return value;
    }

    public async Task SetAsync<T>(string key, T value, TimeSpan? absoluteExpiration = null,
        CancellationToken cancellationToken = default)
    {
        var expiry = absoluteExpiration ?? _options.Expiration;

        memoryCache.Set(key, value, new MemoryCacheEntryOptions()
        {
            AbsoluteExpirationRelativeToNow = expiry,
        });

        var json = JsonSerializer.Serialize(value, _jsonOptions);
        await distributedCache.SetStringAsync(key, json, new DistributedCacheEntryOptions()
        {
            AbsoluteExpirationRelativeToNow = expiry
        }, cancellationToken);
    }

    public async Task RemoveAsync(string key, CancellationToken cancellationToken = default)
    {
        memoryCache.Remove(key);
        await distributedCache.RemoveAsync(key, cancellationToken);
    }

    public async Task<bool> ExistsAsync(string key, CancellationToken cancellationToken = default)
    {
        if (memoryCache.TryGetValue(key, out _))
            return true;

        var json = await distributedCache.GetStringAsync(key, cancellationToken);
        return !string.IsNullOrEmpty(json);
    }
}