using System.Globalization;
using Asp.Versioning;
using BlogApp.Api.BackgroundServices;
using BlogApp.Api.Handlers;
using BlogApp.Api.Localization;
using BlogApp.Api.Options;
using BlogApp.Core.Localization;
using BlogApp.Core.Logging.Extensions;
using BlogApp.Core.Telemetry.Extensions;
using BlogApp.Core.Validations.Abstractions;
using BlogApp.Domain.Options;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;

namespace BlogApp.Api.Extensions;

public static class ServiceCollectionExtensions
{
    extension(IServiceCollection services)
    {
        private IServiceCollection AddExceptionHandler()
        {
            return services.AddExceptionHandler<ExceptionHandler>();
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

        private IServiceCollection AddLocalization()
        {
            services.AddLocalization(options => options.ResourcesPath = "Resources")
                .AddSingleton<IStringLocalizerFactory, JsonStringLocalizerFactory>(sp =>
                {
                    var logger = sp.GetService<ILogger<JsonStringLocalizerFactory>>();
                    return new JsonStringLocalizerFactory(logger!);
                })
                .AddScoped<IValidationMessageLocalizer, ValidationMessageLocalizer>()
                .AddScoped<IErrorMessageLocalizer, ErrorMessageLocalizer>();

            services.ConfigureOptions<RequestLocalizationOptionsSetup>();

            return services;
        }

        private IServiceCollection AddCors()
        {
            return services; //TODO: appsettings'den gelecek
        }

        private IServiceCollection AddLogging()
        {
            services.AddSerilogOptions();

            return services;
        }

        public IServiceCollection AddApiServices(IConfiguration configuration)
        {
            return services
                .AddHttpContextAccessor()
                .AddExceptionHandler()
                .AddAppOptions()
                .AddLogging()
                .AddHostedServices()
                .AddApiVersioning()
                .AddCustomProblemDetails()
                .AddLocalization()
                .AddCors()
                .AddTelemetry(configuration);
        }
    }
}