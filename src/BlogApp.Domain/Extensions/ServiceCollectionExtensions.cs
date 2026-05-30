using BlogApp.Domain.Abstractions.Services;
using BlogApp.Domain.Options;
using BlogApp.Domain.Services;
using Microsoft.Extensions.DependencyInjection;

namespace BlogApp.Domain.Extensions;

public static class ServiceCollectionExtensions
{
    extension(IServiceCollection services)
    {
        private IServiceCollection AddOptions()
        {
            services.ConfigureOptions<KeyRotationOptionsSetup>();
            services.ConfigureOptions<LoginOptionsSetup>();

            return services;
        }

        private IServiceCollection AddServices()
        {
            services.AddOptions()
                .AddScoped<IAuthenticationService, AuthenticationService>()
                .AddScoped<IRefreshTokenService, RefreshTokenService>()
                .AddScoped<ISigningKeyService, SigningKeyService>();
            return services;
        }

        public IServiceCollection AddDomainServices()
        {
            return services.AddServices();
        }
    }
}