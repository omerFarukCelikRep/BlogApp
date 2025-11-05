using Microsoft.Extensions.DependencyInjection;

namespace BlogApp.Domain.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddDomainServices(this IServiceCollection services)
    {
        return services;
    }
}