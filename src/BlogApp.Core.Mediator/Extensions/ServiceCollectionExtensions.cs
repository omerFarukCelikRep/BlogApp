using System.Reflection;
using BlogApp.Core.Mediator.Abstractions;
using BlogApp.Core.Mediator.Handlers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace BlogApp.Core.Mediator.Extensions;

public static class ServiceCollectionExtensions
{
    private static void RegisterHandlers(IServiceProvider provider, ServiceDescriptor service, Registry registry)
    {
        var handlers = provider.GetServices(service.ServiceType);
        foreach (var handler in handlers.Where(s => s is not null))
        {
            var handlerInterface = handler!.GetType().GetInterfaces().First();
            var messagingTypes = handlerInterface.GetGenericArguments();
            typeof(Registry).GetMethods().FirstOrDefault(x =>
                    x.Name == nameof(Registry.AddHandler) && x.GetGenericArguments().Length == messagingTypes.Length)?
                .MakeGenericMethod(messagingTypes).Invoke(registry, [handler]);
        }
    }

    private static void AddHandlers(List<Assembly> assemblies, Type genericType, IServiceCollection services)
    {
        foreach (var handlerType in assemblies.Select(assembly => assembly.ExportedTypes.Where(t =>
                         t.GetInterfaces()
                             .Any(y => y.IsGenericType && (y.GetGenericTypeDefinition() == genericType))))
                     .SelectMany(handlerTypes => handlerTypes))
        {
            var interfaces = handlerType.GetInterface(genericType.Name);
            var interType = genericType.MakeGenericType(interfaces!.GetGenericArguments());

            services.TryAddTransient(interType, handlerType);
        }
    }

    public static IServiceCollection AddMediator(this IServiceCollection services, params List<Assembly> assemblies)
    {
        if (assemblies is not { Count: > 0 })
            assemblies = [Assembly.GetExecutingAssembly()];

        services.AddTransient<IMediator, Handlers.Mediator>();

        AddHandlers(assemblies, typeof(IRequestHandler<>), services);
        AddHandlers(assemblies, typeof(IRequestHandler<,>), services);

        services.AddSingleton<Registry>(provider =>
        {
            var registry = new Registry();
            foreach (var service in services)
            {
                var isHandler = service.ServiceType.GetInterfaces().Any(y => y.IsGenericType &&
                                                                             (y.GetGenericTypeDefinition() ==
                                                                              typeof(IRequestHandler<>) ||
                                                                              y.GetGenericTypeDefinition() ==
                                                                              typeof(IRequestHandler<,>)));
                if (isHandler)
                    RegisterHandlers(provider, service, registry);

                var isBehavior = service.ServiceType.GetInterfaces().Any(y =>
                    y.IsGenericType && (y.GetGenericTypeDefinition() == typeof(IPipelineBehavior<,>)));
                if (isBehavior)
                {
                    var behaviors = provider.GetServices(service.ServiceType);
                    foreach (var behavior in behaviors.Where(s => s is not null))
                    {
                        var behaviorInterface = behavior!.GetType().GetInterfaces().First();
                        var messagingTypes = behaviorInterface.GetGenericArguments();
                        typeof(Registry)
                            .GetMethod(nameof(Registry.AddBehavior))?
                            .MakeGenericMethod(messagingTypes)
                            .Invoke(registry, []);
                    }
                }
            }

            return registry;
        });

        return services;
    }
}