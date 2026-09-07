using System.Diagnostics.Metrics;

namespace BlogApp.Core.Telemetry.Metrics;

public sealed class BlogAppMetrics : IDisposable
{
    public const string MeterName = "BlogApp";

    private readonly Meter _meter;

    // ── HTTP ──
    public readonly Counter<long>   HttpRequestCount;
    public readonly Histogram<double> HttpRequestDuration;

    // ── Blog ──
    public readonly Counter<long>   BlogViewCount;
    public readonly Counter<long>   BlogLikeCount;
    public readonly Counter<long>   BlogCreateCount;
    public readonly Counter<long>   BlogCommentCount;

    // ── Auth ──
    public readonly Counter<long>   LoginSuccessCount;
    public readonly Counter<long>   LoginFailureCount;
    public readonly Counter<long>   RegisterCount;
    public readonly Counter<long>   TokenRefreshCount;

    // ── Cache ──
    public readonly Counter<long>   CacheHitCount;
    public readonly Counter<long>   CacheMissCount;

    // ── Mediator ──
    public readonly Histogram<double> MediatorRequestDuration;
    public readonly Counter<long>     MediatorRequestCount;
    public readonly Counter<long>     MediatorRequestFailureCount;

    public BlogAppMetrics()
    {
        _meter = new Meter(MeterName, "1.0.0");

        // HTTP
        HttpRequestCount = _meter.CreateCounter<long>(
            "blogapp.http.requests.total",
            description: "Total HTTP requests");

        HttpRequestDuration = _meter.CreateHistogram<double>(
            "blogapp.http.request.duration",
            unit: "ms",
            description: "HTTP request duration in milliseconds");

        // Blog
        BlogViewCount = _meter.CreateCounter<long>(
            "blogapp.blog.views.total",
            description: "Total blog post views");

        BlogLikeCount = _meter.CreateCounter<long>(
            "blogapp.blog.likes.total",
            description: "Total blog post likes");

        BlogCreateCount = _meter.CreateCounter<long>(
            "blogapp.blog.created.total",
            description: "Total blog posts created");

        BlogCommentCount = _meter.CreateCounter<long>(
            "blogapp.blog.comments.total",
            description: "Total blog comments created");

        // Auth
        LoginSuccessCount = _meter.CreateCounter<long>(
            "blogapp.auth.login.success.total",
            description: "Total successful logins");

        LoginFailureCount = _meter.CreateCounter<long>(
            "blogapp.auth.login.failure.total",
            description: "Total failed logins");

        RegisterCount = _meter.CreateCounter<long>(
            "blogapp.auth.register.total",
            description: "Total registrations");

        TokenRefreshCount = _meter.CreateCounter<long>(
            "blogapp.auth.token.refresh.total",
            description: "Total token refreshes");

        // Cache
        CacheHitCount = _meter.CreateCounter<long>(
            "blogapp.cache.hit.total",
            description: "Total cache hits");

        CacheMissCount = _meter.CreateCounter<long>(
            "blogapp.cache.miss.total",
            description: "Total cache misses");

        // Mediator
        MediatorRequestDuration = _meter.CreateHistogram<double>(
            "blogapp.mediator.request.duration",
            unit: "ms",
            description: "Mediator request duration in milliseconds");

        MediatorRequestCount = _meter.CreateCounter<long>(
            "blogapp.mediator.requests.total",
            description: "Total mediator requests");

        MediatorRequestFailureCount = _meter.CreateCounter<long>(
            "blogapp.mediator.requests.failed.total",
            description: "Total failed mediator requests");
    }

    public void Dispose() => _meter.Dispose();
}