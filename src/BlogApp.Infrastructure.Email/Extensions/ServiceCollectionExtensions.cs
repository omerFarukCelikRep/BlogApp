using BlogApp.Core.Email.Abstractions;
using BlogApp.Infrastructure.Email.Services;
using Microsoft.Extensions.DependencyInjection;

namespace BlogApp.Infrastructure.Email.Extensions;

public static class ServiceCollectionExtensions
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddEmailServices()
        {
            return services
                .AddSingleton<IEmailTemplateService, EmailTemplateService>();
        }
    }
}