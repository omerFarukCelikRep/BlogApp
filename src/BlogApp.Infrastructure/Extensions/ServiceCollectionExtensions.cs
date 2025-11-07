using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using BlogApp.Infrastructure.EFCore.Extensions;

namespace BlogApp.Infrastructure.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddHttpContextAccessor()
            .AddEFCoreServices(configuration)
            .AddAuthorization();

        return services;
    }
}