using System.Globalization;
using Asp.Versioning;
using BlogApp.Api.BackgroundServices;
using BlogApp.Api.Handlers;
using BlogApp.Api.Localization;
using BlogApp.Api.Options;
using BlogApp.Core.Validations.Abstractions;
using Microsoft.Extensions.Localization;

namespace BlogApp.Api.Extensions;

public static class ServiceCollectionExtensions
{
    extension(IServiceCollection services)
    {
        private IServiceCollection AddExceptionHandler()
        {
            return services.AddExceptionHandler<ExceptionHandler>();
        }

        private IServiceCollection AddControllersServices()
        {
            services.AddControllers(options => options.Filters.Add<TimingActionFilter>())
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                    options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault;
                });
            return services;
        }

        private IServiceCollection AddApiVersioning()
        {
            services.AddApiVersioning(options =>
            {
                options.DefaultApiVersion = new ApiVersion(1, 0);
                options.AssumeDefaultVersionWhenUnspecified = true;
                options.ReportApiVersions = true;
                options.ApiVersionReader = ApiVersionReader.Combine(
                    new UrlSegmentApiVersionReader(),
                    new HeaderApiVersionReader());
            });

            return services;
        }

        private IServiceCollection AddAppOptions()
        {
            services.ConfigureOptions<KeyRotationOptionsSetup>();
            services.ConfigureOptions<CultureOptionsSetup>();

            return services;
        }

        private IServiceCollection AddHostedServices()
        {
            services.AddHostedService<KeyRotationBackgroundService>();

            return services;
        }

        private IServiceCollection AddCustomProblemDetails()
        {
            services.AddProblemDetails();

            return services;
        }

        public IServiceCollection AddApiServices()
        {
            return services
                .AddHttpContextAccessor()
                .AddExceptionHandler()
                .AddAppOptions()
                .AddHostedServices()
                .AddApiVersioning()
                .AddCustomProblemDetails();
        }
    }
}