using System.Reflection;
using BlogApp.Core.Mediator.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace BlogApp.Core.Mediator.Extensions;

public static class ServiceCollectionExtensions
{
    private static void AddHandlers(Assembly[] assemblies, Type genericType, IServiceCollection services)
    {
        foreach (var handlerType in assemblies.Select(assembly => assembly!.ExportedTypes.Where(t =>
                         t.GetInterfaces()
                             .Any(y => y.IsGenericType && (y.GetGenericTypeDefinition() == genericType))))
                     .SelectMany(handlerTypes => handlerTypes))
        {
            var interfaces = handlerType.GetInterface(genericType.Name);
            var interType = genericType.MakeGenericType(interfaces!.GetGenericArguments());

            services.TryAddTransient(interType, handlerType);
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