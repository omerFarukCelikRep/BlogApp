using System.Reflection;
using BlogApp.Core.Mediator.Behaviors;
using BlogApp.Core.Mediator.Extensions;
using BlogApp.Core.Validations.Extensions;
using Microsoft.Extensions.DependencyInjection;

namespace BlogApp.Application.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddMediator(Assembly.GetExecutingAssembly())
            .AddValidators(Assembly.GetExecutingAssembly())
            .AddTransient(typeof(IPipelineBehavior<,>), typeof(RequestHandlerPreProcesserBehavior<,>))
            .AddTransient(typeof(IPipelineBehavior<,>), typeof(RequestHandlerPostProcesserBehavior<,>))
            .AddTransient(typeof(IPipelineBehavior<,>), typeof(AuthorizeBehavior<,>))
            .AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>))
            .AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
        // .AddTransient(typeof(IPipelineBehavior<,>), typeof(CachingBehavior<,>)); //TODO

        return services;
    }
}