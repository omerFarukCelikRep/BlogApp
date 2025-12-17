using System.Reflection;
using BlogApp.Core.Validations.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace BlogApp.Core.Validations.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddValidators(this IServiceCollection services, params Assembly[] assemblies)
    {
        if (assemblies is not { Length: > 0 })
            assemblies = [Assembly.GetExecutingAssembly()];

        var type = typeof(IValidator<>);
        foreach (var handlerType in assemblies.Select(assembly => assembly!.ExportedTypes.Where(t =>
                         t.GetInterfaces()
                             .Any(y => y.IsGenericType && (y.GetGenericTypeDefinition() == type))))
                     .SelectMany(handlerTypes => handlerTypes))
        {
            var interfaces = handlerType.GetInterface(type.Name);
            var interType = type.MakeGenericType(interfaces!.GetGenericArguments());

            services.TryAddTransient(interType, handlerType);
        }

        return services;
    }
}