using System.Reflection;
using BlogApp.Core.Caching.Behaviors;
using BlogApp.Core.Logging.Behaviors;
using BlogApp.Core.Mediator.Behaviors;
using BlogApp.Core.Mediator.Extensions;
using BlogApp.Core.Security.Behaviors;
using BlogApp.Core.Validations.Behaviors;
using BlogApp.Core.Validations.Extensions;
using Microsoft.Extensions.DependencyInjection;

namespace BlogApp.Application.Extensions;

public static class ServiceCollectionExtensions
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddApplicationServices()
        {
            services.AddMediator(Assembly.GetExecutingAssembly())
                .AddValidators(Assembly.GetExecutingAssembly())
                .AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>))
                .AddTransient(typeof(IPipelineBehavior<,>), typeof(RequestHandlerPreProcessorBehavior<,>))
                .AddTransient(typeof(IPipelineBehavior<,>), typeof(RequestHandlerPostProcessorBehavior<,>))
                .AddTransient(typeof(IPipelineBehavior<,>), typeof(AuthorizationBehavior<,>))
                .AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>))
                .AddTransient(typeof(IPipelineBehavior<,>), typeof(CachingBehavior<,>));

            return services;
        }
    }
}