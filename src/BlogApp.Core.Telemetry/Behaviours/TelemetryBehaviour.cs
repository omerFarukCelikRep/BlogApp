using System.Diagnostics;
using BlogApp.Core.Telemetry.Abstractions;
using BlogApp.Core.Telemetry.Metrics;
using Microsoft.Extensions.Logging;
using BlogApp.Core.Mediator.Abstractions;
using BlogApp.Core.Telemetry.Services;

namespace BlogApp.Core.Telemetry.Behaviours;

public class TelemetryBehaviour<TRequest, TResponse>(
    ITelemetryService telemetryService,
    BlogAppMetrics blogAppMetrics,
    ILogger<TelemetryBehaviour<TRequest, TResponse>> logger)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken = default)
    {
        var requestName = typeof(TRequest).Name;
        var assemblyName = typeof(TRequest).Assembly.GetName().Name;

        using var activity = TelemetryService.ActivitySource.StartActivity($"Mediator/{requestName}",
            ActivityKind.Internal, assemblyName);

        activity?.SetTag("mediator.request.type", requestName);
        activity?.SetTag("mediator.request.assembly", assemblyName);

        var stopwatch = Stopwatch.StartNew();
        try
        {
            var response = await next(cancellationToken);

            stopwatch.Stop();

            blogAppMetrics.MediatorRequestCount.Add(1,
                KeyValuePair.Create<string, object?>("request.name", requestName),
                KeyValuePair.Create<string, object?>("request.success", true));

            blogAppMetrics.MediatorRequestDuration.Record(stopwatch.Elapsed.TotalMilliseconds,
                KeyValuePair.Create<string, object?>("request.name", requestName));

            activity?.SetStatus(ActivityStatusCode.Ok);

            logger.LogDebug(
                "Mediator request {RequestName} completed in {Duration}ms",
                requestName,
                stopwatch.ElapsedMilliseconds);

            return response;
        }
        catch (Exception ex)
        {
            stopwatch.Stop();

            blogAppMetrics.MediatorRequestCount.Add(1,
                KeyValuePair.Create<string, object?>("request.name", requestName),
                KeyValuePair.Create<string, object?>("request.success", false));

            blogAppMetrics.MediatorRequestFailureCount.Add(1,
                KeyValuePair.Create<string, object?>("request.name", requestName),
                KeyValuePair.Create<string, object?>("exception.type", ex.GetType().Name));

            blogAppMetrics.MediatorRequestDuration.Record(
                stopwatch.Elapsed.TotalMilliseconds,
                KeyValuePair.Create<string, object?>("request.name", requestName));

            telemetryService.SetError(activity, ex);

            logger.LogError(ex,
                "Mediator request {RequestName} failed after {Duration}ms",
                requestName,
                stopwatch.ElapsedMilliseconds);

            throw;
        }
    }
}