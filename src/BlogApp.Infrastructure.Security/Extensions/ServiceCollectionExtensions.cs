using BlogApp.Core.Security.Abstractions;
using BlogApp.Infrastructure.Security.Providers;
using BlogApp.Infrastructure.Security.Setups;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BlogApp.Infrastructure.Security.Extensions;

public static class ServiceCollectionExtensions
{
    private static IServiceCollection AddAuthorization(this IServiceCollection services)
    {
        services.ConfigureOptions<JwtOptionsSetup>();
        services.ConfigureOptions<JwtBearerOptionsSetup>();

        services.AddScoped<IDomainPrincipal, DomainPrincipal>();
        services.AddSingleton<IJwtProvider, JwtProvider>();
        services.AddScoped<IRoleManager, RoleManager>();
        services.AddSingleton<IAuthorizationManager, AuthorizationManager>();

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme);
        return services;
    }

    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddAuthorization();

        return services;
    }
}