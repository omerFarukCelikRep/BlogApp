using System.Reflection;
using BlogApp.Core.Mediator.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace BlogApp.Core.Mediator.Extensions;

public static class ServiceCollectionExtensions
{
    private static void AddHandlers(Assembly[] assemblies, Type genericType, IServiceCollection services)
    {
        var handlerTypes = assemblies.SelectMany(x => x.ExportedTypes)
            .Where(x => x.GetInterfaces().Any(i =>  i.IsGenericType && i.GetGenericTypeDefinition() == genericType));
        foreach (var handlerType in handlerTypes)
        {
            var interfaceType = handlerType.GetInterfaces()
                .First(x => x.IsGenericType && x.GetGenericTypeDefinition() == genericType);
            
            var serviceType = genericType.MakeGenericType(interfaceType.GetGenericArguments());
            services.TryAddTransient(serviceType, handlerType);
        }
    }

    public static IServiceCollection AddMediator(this IServiceCollection services, params Assembly[] assemblies)
    {
        if (assemblies is not { Length: > 0 })
            assemblies = [Assembly.GetExecutingAssembly()];

        services.AddTransient<IMediator, Handlers.Mediator>();

        AddHandlers(assemblies, typeof(IRequestHandler<>), services);
        AddHandlers(assemblies, typeof(IRequestHandler<,>), services);

        return services;
    }
}