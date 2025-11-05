using System.Reflection;
using BlogApp.Core.Mediator.Abstractions;
using BlogApp.Core.Mediator.Handlers;
using Microsoft.Extensions.DependencyInjection;

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
            typeof(Registry)
                .GetMethod(nameof(Registry.AddHandler))?
                .MakeGenericMethod(messagingTypes)
                .Invoke(registry, [handler]);
        }
    }

    public static IServiceCollection AddMediator(this IServiceCollection services, params List<Assembly> assemblies)
    {
        if (assemblies is not { Count: > 0 })
            assemblies = [Assembly.GetExecutingAssembly()];

        services.AddSingleton<IMediator, Handlers.Mediator>();

        foreach (var handlerType in assemblies.Select(assembly => assembly.ExportedTypes.Where(t =>
                         t.GetInterfaces()
                             .Any(y => y.IsGenericType && (y.GetGenericTypeDefinition() == typeof(IRequestHandler<>) ||
                                                           y.GetGenericTypeDefinition() ==
                                                           typeof(IRequestHandler<,>)))))
                     .SelectMany(handlerTypes => handlerTypes))
        {
            services.AddTransient(handlerType);
        }

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


    public static IServiceCollection AddMediatorAlt(this IServiceCollection services, params List<Assembly> assemblies)
    {
        if (assemblies is not { Count: > 0 })
            assemblies = [Assembly.GetExecutingAssembly()];

        services.AddSingleton<IMediator, Handlers.Mediator>();

        var mediatorTypes = assemblies.SelectMany(x => x.GetTypes())
            .Where(x => x is { IsAbstract: false, IsInterface: false })
            .SelectMany(x => x.GetInterfaces(), (impl, iface) => new { impl, iface })
            .Where(x => x.iface.IsGenericType && (x.iface.GetGenericTypeDefinition() == typeof(IRequestHandler<,>) ||
                                                  x.iface.GetGenericTypeDefinition() == typeof(IPipelineBehavior<,>)))
            .ToList();

        var handlerTypes = mediatorTypes.Where(x => x.iface.GetGenericTypeDefinition() == typeof(IRequestHandler<,>));

        foreach (var handlerType in handlerTypes)
        {
            services.AddScoped(handlerType.iface, handlerType.impl);
        }

        var behaviorTypes =
            mediatorTypes.Where(x => x.iface.GetGenericTypeDefinition() == typeof(IPipelineBehavior<,>));

        foreach (var behaviorType in behaviorTypes)
        {
            services.AddScoped(behaviorType.iface, behaviorType.impl);
        }

        return services;
    }
}