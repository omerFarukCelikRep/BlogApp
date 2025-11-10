using BlogApp.Core.Logging.Contexts;
using BlogApp.Core.Logging.Options;
using Elastic.Channels;
using Elastic.CommonSchema.Serilog;
using Elastic.Ingest.Elasticsearch;
using Elastic.Ingest.Elasticsearch.DataStreams;
using Elastic.Serilog.Sinks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;

namespace BlogApp.Core.Logging.Extensions;

public static class HostBuilderExtensions
{
    private static LogEventLevel GetMinimumLevel(SerilogOptions options)
    {
        return options.MinimumLevel.Default.ToLower() switch
        {
            "debug" => LogEventLevel.Debug,
            "warning" => LogEventLevel.Warning,
            "error" => LogEventLevel.Error,
            _ => LogEventLevel.Information
        };
    }

    private static void ConfigureElasticsearchSink(ElasticsearchSinkOptions opts, LogEventLevel minimumLevel)
    {
        opts.TextFormatting = new EcsTextFormatterConfiguration<LogEventEcsDocument>();
        opts.DataStream = new DataStreamName("logs", "BlogApp");
        ;
        opts.BootstrapMethod = BootstrapMethod.Failure;
        opts.MinimumLevel = minimumLevel;
        opts.ConfigureChannel = channel => channel.BufferOptions = new BufferOptions() { ExportMaxConcurrency = 10 };
    }

    public static IHostBuilder UseSerilog(this IHostBuilder hostBuilder)
    {
        hostBuilder.UseSerilog((context, configuration) =>
        {
            var options = context.Configuration.GetSection(SerilogOptions.Section).Get<SerilogOptions>();
            ArgumentNullException.ThrowIfNull(options);

            configuration.ReadFrom.Configuration(context.Configuration)
                .MinimumLevel
                .Override("Serilog.AspNetCore.RequestLoggingMiddleware", GetMinimumLevel(options))
                .Enrich.WithProperty(CorrelationContext.CorrelationPropertyName, CorrelationContext.CurrentId)
                //TODO: .Enrich.WithEcsHttpContext(context.Configuration.Get<IHttpContextAccessor>()) 
                .WriteTo.Conditional(x => options.File!.Enabled == true, opts =>
                {
                    var fileOptions = options.File!;
                    opts.File(path: fileOptions.Path,
                        rollingInterval: Enum.Parse<RollingInterval>(fileOptions.RollingInterval),
                        outputTemplate: fileOptions.OutputTemplate,
                        rollOnFileSizeLimit: fileOptions.RollOnFileSizeLimit);
                });
            // .WriteTo.Conditional(x => options.Elastic!.Enabled, opts =>
            // {
            //     var elasticOptions = options.Elastic;
            //     var uris = elasticOptions!.Urls.Select(uri => new Uri(uri)).ToList();
            //     opts.Elasticsearch(uris, opt => ConfigureElasticsearchSink(opt, GetMinimumLevel(options)));
            // });
        });

        return hostBuilder;
    }
}