using System.Data;
using BlogApp.Core.Telemetry.Abstractions;
using BlogApp.Core.Telemetry.Metrics;
using BlogApp.Core.Telemetry.Options;
using BlogApp.Core.Telemetry.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using OpenTelemetry.Metrics;

namespace BlogApp.Core.Telemetry.Extensions;

public static class ServiceCollectionExtensions
{
    extension(IServiceCollection services)
    {
        /// <summary>
        /// </summary>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public IServiceCollection AddTelemetry(IConfiguration configuration)
        {
            var optionsSection = configuration.GetSection(TelemetryOptions.Section);
            var options = optionsSection.Get<TelemetryOptions>()
                          ?? new TelemetryOptions();
            if (!options.Enabled)
                return services;

            services.Configure<TelemetryOptions>(optionsSection);

            services.AddSingleton<BlogAppMetrics>();
            services.AddSingleton<ITelemetryService, TelemetryService>();

            var resource = ResourceBuilder.CreateDefault()
                .AddService(options.ServiceName, options.ServiceVersion)
                .AddAttributes(new Dictionary<string, object>()
                {
                    ["deployment.environment"] = options.Environment
                });

            services.AddOpenTelemetry()
                .WithTracing(tracing =>
                {
                    tracing.SetResourceBuilder(resource)
                        .AddAspNetCoreInstrumentation(opts =>
                        {
                            opts.RecordException = true;
                            opts.Filter = ctx =>
                                !ctx.Request.Path.StartsWithSegments("/health") &&
                                !ctx.Request.Path.StartsWithSegments("/metrics");
                        })
                        .AddHttpClientInstrumentation(opts => opts.RecordException = true)
                        .AddEntityFrameworkCoreInstrumentation(opts => opts.Filter = (_, command) =>
                            command.CommandType == CommandType.StoredProcedure ||
                            command.CommandType == CommandType.Text)
                        .AddRedisInstrumentation()
                        .AddSource(TelemetryService.ActivitySource.Name);

                    if (options.Jaeger.Enabled)
                        tracing.AddOtlpExporter(opts => opts.Endpoint = new Uri(options.Jaeger.Endpoint));
                })
                .WithMetrics(metrics =>
                {
                    metrics.SetResourceBuilder(resource)
                        .AddAspNetCoreInstrumentation()
                        .AddHttpClientInstrumentation()
                        .AddRuntimeInstrumentation()
                        .AddMeter(BlogAppMetrics.MeterName);

                    if (options.Prometheus.Enabled)
                        metrics.AddPrometheusExporter();
                });

            return services;
        }
    }
}