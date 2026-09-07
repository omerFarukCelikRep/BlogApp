using System.Diagnostics;
using BlogApp.Core.Telemetry.Abstractions;
using BlogApp.Core.Telemetry.Metrics;

namespace BlogApp.Core.Telemetry.Services;

public class TelemetryService(BlogAppMetrics blogAppMetrics) : ITelemetryService
{
    public static readonly ActivitySource ActivitySource = new("BlogApp", "1.0.0");

    private static string ExtractPrefix(string key)
    {
        var index = key.IndexOf(':');
        return index > 0 ? key[..index] : key;
    }

    //Spans
    public Activity? StartActivity(string name, ActivityKind kind = ActivityKind.Internal) =>
        ActivitySource.StartActivity(name, kind);

    public void SetTag(Activity? activity, string key, object? value) => activity?.SetTag(key, value);

    public void SetError(Activity? activity, Exception exception)
    {
        activity?.SetStatus(ActivityStatusCode.Error, exception.Message);
        activity?.SetTag("exception.type", exception.GetType().Name);
        activity?.SetTag("exception.message", exception.Message);
        activity?.SetTag("exception.stacktrace", exception.StackTrace);
    }

    public void SetStatus(Activity? activity, bool success) =>
        activity?.SetStatus(success ? ActivityStatusCode.Ok : ActivityStatusCode.Error);

    // BLOG METRICS
    public void RecordBlogView(int blogId, string title)
    {
        blogAppMetrics.BlogViewCount.Add(
            1,
            KeyValuePair.Create<string, object?>("blog.id", blogId),
            KeyValuePair.Create<string, object?>("blog.title", title));
    }

    public void RecordBlogLike(int blogId, bool isLike)
    {
        blogAppMetrics.BlogLikeCount.Add(
            1,
            KeyValuePair.Create<string, object?>("blog.id", blogId),
            KeyValuePair.Create<string, object?>("like.action", isLike ? "like" : "unlike"));
    }

    public void RecordBlogCreate(string postStatus)
    {
        blogAppMetrics.BlogCreateCount.Add(
            1,
            KeyValuePair.Create<string, object?>("post.status", postStatus));
    }

    public void RecordBlogComment(int blogId)
    {
        blogAppMetrics.BlogCommentCount.Add(
            1,
            KeyValuePair.Create<string, object?>("blog.id", blogId));
    }

    // AUTH METRICS
    public void RecordLoginSuccess(string email)
    {
        blogAppMetrics.LoginSuccessCount.Add(1,
            KeyValuePair.Create<string, object?>("auth.method", "email"));
    }

    public void RecordLoginFailure(string email, string reason)
    {
        blogAppMetrics.LoginFailureCount.Add(1,
            KeyValuePair.Create<string, object?>("auth.failure.reason", reason));
    }

    public void RecordRegister() => blogAppMetrics.RegisterCount.Add(1);

    public void RecordTokenRefresh() => blogAppMetrics.TokenRefreshCount.Add(1);

    // CACHE METRICS
    public void RecordCacheHit(string cacheKey)
    {
        blogAppMetrics.CacheHitCount.Add(1,
            KeyValuePair.Create<string, object?>("cache.key.prefix", ExtractPrefix(cacheKey)));
    }

    public void RecordCacheMiss(string cacheKey)
    {
        blogAppMetrics.CacheMissCount.Add(1,
            KeyValuePair.Create<string, object?>("cache.key.prefix", ExtractPrefix(cacheKey)));
    }
}