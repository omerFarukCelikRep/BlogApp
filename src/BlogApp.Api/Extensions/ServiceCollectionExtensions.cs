using System.Text.Json.Serialization;
using Asp.Versioning;
using BlogApp.Api.BackgroundServices;
using BlogApp.Api.Filters;
using BlogApp.Api.Handlers;
using BlogApp.Api.Options;

namespace BlogApp.Api.Extensions;

public static class ServiceCollectionExtensions
{
    private static IServiceCollection AddExceptionHandler(this IServiceCollection services)
    {
        return services.AddExceptionHandler<ExceptionHandler>();
    }

    private static IServiceCollection AddControllersServices(this IServiceCollection services)
    {
        services.AddControllers(options => options.Filters.Add<TimingActionFilter>())
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault;
            });
        return services;
    }

    private static IServiceCollection AddApiVersioning(this IServiceCollection services)
    {
        services.AddApiVersioning(options =>
        {
            options.DefaultApiVersion = new ApiVersion(1, 0);
            options.AssumeDefaultVersionWhenUnspecified = true;
            options.ReportApiVersions = true;
        });

        return services;
    }

    private static IServiceCollection AddOptions(this IServiceCollection services, IConfiguration configuration)
    {
        configuration.GetSection(KeyRotationOptions.SectionName).Bind(configuration);

        return services;
    }

    private static IServiceCollection AddHostedServices(this IServiceCollection services)
    {
        services.AddHostedService<KeyRotationBackgroundService>();

        return services;
    }

    private static IServiceCollection AddCustomProblemDetails(this IServiceCollection services)
    {
        services.AddProblemDetails();

        return services;
    }

    public static IServiceCollection AddApiServices(this IServiceCollection services)
    {
        return services.AddExceptionHandler()
            .AddOptions()
            .AddHostedServices()
            // .AddControllersServices()
            .AddApiVersioning()
            .AddCustomProblemDetails();
    }
}