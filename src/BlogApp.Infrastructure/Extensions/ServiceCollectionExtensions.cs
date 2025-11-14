using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using BlogApp.Infrastructure.EFCore.Extensions;
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
                .AddSecurityServices(configuration);

            return services;
        }
    }
}