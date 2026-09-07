using System.Diagnostics;

namespace BlogApp.Core.Telemetry.Abstractions;

public interface ITelemetryService
{
    // ── Spans ──
    Activity? StartActivity(
        string name,
        ActivityKind kind = ActivityKind.Internal);

    void SetTag(Activity? activity, string key, object? value);
    void SetError(Activity? activity, Exception exception);
    void SetStatus(Activity? activity, bool success);

    // ── Blog metrics ──
    void RecordBlogView(int blogId, string title);
    void RecordBlogLike(int blogId, bool isLike);
    void RecordBlogCreate(string postStatus);
    void RecordBlogComment(int blogId);

    // ── Auth metrics ──
    void RecordLoginSuccess(string email);
    void RecordLoginFailure(string email, string reason);
    void RecordRegister();
    void RecordTokenRefresh();

    // ── Cache metrics ──
    void RecordCacheHit(string cacheKey);
    void RecordCacheMiss(string cacheKey);
}