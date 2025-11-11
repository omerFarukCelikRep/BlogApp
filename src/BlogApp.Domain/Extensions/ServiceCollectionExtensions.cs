using BlogApp.Domain.Abstractions.Services;
using BlogApp.Domain.Services;
using Microsoft.Extensions.DependencyInjection;

namespace BlogApp.Domain.Extensions;

public static class ServiceCollectionExtensions
{
    extension(IServiceCollection services)
    {
        private IServiceCollection AddServices()
        {
            services.AddScoped<IAuthenticationService, AuthenticationService>();
            return services;
        }

        public IServiceCollection AddDomainServices()
        {
            services.AddServices();

            return services;
        }
    }
}