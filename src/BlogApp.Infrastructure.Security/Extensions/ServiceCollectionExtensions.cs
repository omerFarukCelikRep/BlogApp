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
        private IServiceCollection AddAuthentication()
        {
            services.ConfigureOptions<JwtOptionsSetup>();
            services.ConfigureOptions<JwtBearerOptionsSetup>();

            services.AddScoped<IDomainPrincipal, DomainPrincipal>()
                .AddScoped<IJwtProvider, JwtProvider>()
                .AddScoped<IRoleManager, RoleManager>()
                .AddScoped<IAuthorizationManager, AuthorizationManager>()
                .AddScoped<IRefreshTokenProvider, RefreshTokenProvider>();

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme);

            services.AddAuthorization();
            return services;
        }

        public IServiceCollection AddSecurityServices(IConfiguration configuration)
        {
            services.AddAuthentication();

            return services;
        }
    }
}