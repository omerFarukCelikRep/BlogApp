using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using BlogApp.Infrastructure.EFCore.Extensions;

namespace BlogApp.Infrastructure.Extensions;

public static class ServiceCollectionExtensions
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddInfrastructureServices(IConfiguration configuration)
        {
            services.AddHttpContextAccessor()
                .AddEFCoreServices(configuration)
                .AddAuthorization();

            return services;
        }
    }
}