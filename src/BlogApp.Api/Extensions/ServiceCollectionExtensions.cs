using System.Globalization;
using Asp.Versioning;
using BlogApp.Api.BackgroundServices;
using BlogApp.Api.Handlers;
using BlogApp.Api.Localization;
using BlogApp.Api.Options;
using BlogApp.Core.Localization;
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
                    return new(logger!);
                })
                .AddScoped<IValidationMessageLocalizer, ValidationMessageLocalizer>()
                .AddScoped<IErrorMessageLocalizer, ErrorMessageLocalizer>();

            services.Configure<RequestLocalizationOptions>(options =>
            {
                var cultureOptions = services.BuildServiceProvider().GetRequiredService<IOptions<CultureOptions>>()
                    .Value;

                var supportedCultures = cultureOptions.Supported.Select(x => new CultureInfo(x)).ToList();

                options.DefaultRequestCulture = new RequestCulture(cultureOptions.Default);
                options.SupportedCultures = supportedCultures;
                options.SupportedUICultures = supportedCultures;
            });

            return services;
        }

        private IServiceCollection AddCors()
        {
            return services; //TODO: appsettings'den gelecek
        }

        public IServiceCollection AddApiServices()
        {
            return services
                .AddHttpContextAccessor()
                .AddExceptionHandler()
                .AddAppOptions()
                .AddHostedServices()
                .AddApiVersioning()
                .AddCustomProblemDetails()
                .AddLocalization()
                .AddCors();
        }
    }
}