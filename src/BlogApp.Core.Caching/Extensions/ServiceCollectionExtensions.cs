using BlogApp.Core.Caching.Enums;
using BlogApp.Core.Caching.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BlogApp.Core.Caching.Extensions;

public static class ServiceCollectionExtensions
{
    private static void ValidateSettings(CacheOptions options)
    {
        if (options.Provider != CacheProvider.Memory && options.Redis is null)
            throw new InvalidOperationException(
                "Redis settings must be configured when Provider is Redis or Hybrid.");

        if (options.Provider != CacheProvider.Memory && string.IsNullOrWhiteSpace(options.Redis?.ConnectionString))
            throw new InvalidOperationException("Redis ConnectionString cannot be empty.");
    }

    extension(IServiceCollection services)
    {
        public IServiceCollection AddCoreCaching(IConfiguration configuration,
            Action<CacheOptions> configure)
        {
            var section = configuration.GetSection(CacheOptions.SectionName);
            var options = section.Get<CacheOptions>()
                          ?? new CacheOptions();

            services.Configure<CacheOptions>(section);

            ValidateSettings(options);

            configure(options);

            return services;
        }
    }
}