using System.Text.Json.Serialization;
using Asp.Versioning;
using BlogApp.Api.Filters;
using BlogApp.Api.Handlers;
using Microsoft.AspNetCore.Authentication.JwtBearer;

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

    public static IServiceCollection AddApiServices(this IServiceCollection services)
    {
        return services.AddExceptionHandler()
            .AddControllersServices()
            .AddApiVersioning();
    }
}