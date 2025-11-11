using BlogApp.Core.Security.Abstractions;
using BlogApp.Infrastructure.Security.Providers;
using BlogApp.Infrastructure.Security.Setups;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BlogApp.Infrastructure.Security.Extensions;

public static class ServiceCollectionExtensions
{
    extension(IServiceCollection services)
    {
        private IServiceCollection AddAuthorization()
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

        public IServiceCollection AddInfrastructureServices(IConfiguration configuration)
        {
            services.AddAuthorization();

            return services;
        }
    }
}