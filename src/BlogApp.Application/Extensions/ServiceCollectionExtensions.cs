using System.Reflection;
using BlogApp.Core.Mediator.Extensions;
using Microsoft.Extensions.DependencyInjection;

namespace BlogApp.Application.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddMediator(Assembly.GetExecutingAssembly());
        return services;
    }
}