using BlogApp.Core.EFCore.Interceptors;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BlogApp.Core.EFCore.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddScopedInterceptors(this IServiceCollection services)
    {
        //TODO:appsettings üzerinden belirtilen sınıfllar dahil edilecek
        services.AddScoped<QueryTimingInterceptor>();
        services.AddScoped<SaveAuditableChangesInterceptor>();
        services.AddScoped<SqlLoggingInterceptor>();
        
        return services;
    }
}