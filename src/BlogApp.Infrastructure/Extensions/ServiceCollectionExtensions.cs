using BlogApp.Infrastructure.Caching.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using BlogApp.Infrastructure.EFCore.Extensions;
using BlogApp.Infrastructure.Email.Extensions;
using BlogApp.Infrastructure.Search.Extensions;
using BlogApp.Infrastructure.Security.Extensions;

namespace BlogApp.Infrastructure.Extensions;

public static class ServiceCollectionExtensions
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddInfrastructureServices(IConfiguration configuration)
        {
            services.AddHttpContextAccessor()
                .AddEFCoreServices(configuration)
                .AddSecurityServices()
                .AddCachingServices(configuration)
                .AddSearch(configuration)
                .AddEmailServices();

            return services;
        }
    }
}