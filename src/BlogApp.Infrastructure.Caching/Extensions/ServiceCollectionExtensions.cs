using BlogApp.Core.Caching.Abstractions;
using BlogApp.Core.Caching.Enums;
using BlogApp.Core.Caching.Extensions;
using BlogApp.Core.Caching.Options;
using BlogApp.Infrastructure.Caching.Services;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace BlogApp.Infrastructure.Caching.Extensions;

public static class ServiceCollectionExtensions
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddCachingServices(IConfiguration configuration)
        {
            services.AddCoreCaching(configuration, options =>
            {
                if (options.Provider is CacheProvider.Memory or CacheProvider.Hybrid)
                    services.AddMemoryCache();

                if (options.Provider is CacheProvider.Redis or CacheProvider.Hybrid)
                    services.AddStackExchangeRedisCache(opts =>
                    {
                        opts.Configuration = options.Redis!.ConnectionString;
                        opts.InstanceName = options.Redis.InstanceName;
                    });

                services.AddScoped<ICacheService>(options.Provider switch
                {
                    CacheProvider.Memory => sp => new InMemoryCacheService(sp.GetRequiredService<IMemoryCache>(),
                        sp.GetRequiredService<IOptions<CacheOptions>>()),
                    CacheProvider.Redis => sp => new RedisCacheService(sp.GetRequiredService<IDistributedCache>(),
                        sp.GetRequiredService<IOptions<CacheOptions>>()),
                    CacheProvider.Hybrid => sp => new HybridCacheService(sp.GetRequiredService<IMemoryCache>(),
                        sp.GetRequiredService<IDistributedCache>(),
                        sp.GetRequiredService<IOptions<CacheOptions>>()),
                    _ => throw new ArgumentOutOfRangeException()
                });
            });

            return services;
        }
    }
}