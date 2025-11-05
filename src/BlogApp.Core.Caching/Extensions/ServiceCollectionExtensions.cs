using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.DependencyInjection;

namespace BlogApp.Core.Caching.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddHybridCache(this IServiceCollection services)
    {
        services.AddHybridCache(options =>
        {
            options.MaximumPayloadBytes = 1024 * 10 * 10;
            options.MaximumKeyLength = 256;
            options.DefaultEntryOptions = new HybridCacheEntryOptions()
            {
                Expiration = TimeSpan.FromMinutes(10),
                LocalCacheExpiration = TimeSpan.FromMinutes(10)
            };
            options.ReportTagMetrics = true;
            options.DisableCompression = false;
        });

        return services;
    }
}