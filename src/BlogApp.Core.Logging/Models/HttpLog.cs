namespace BlogApp.Core.Logging.Models;

/// <summary>
/// Represents the log data for an incoming API request.
/// </summary>
public class HttpLog(string traceId, string path, string method)
{
    private static readonly HashSet<string> _safeHeaders = new(StringComparer.OrdinalIgnoreCase)
    {
        "Content-Type", "Accept", "X-Correlation-Id", "User-Agent"
    };

    /// <summary>
    /// Unique trace identifier for correlating logs.
    /// </summary>
    public string TraceId { get; set; } = traceId;

    /// <summary>
    /// The full request path (e.g., /api/posts/1).
    /// </summary>
    public string Path { get; set; } = path;

    /// <summary>
    /// The HTTP method (GET, POST, etc.).
    /// </summary>
    public string Method { get; set; } = method;

    /// <summary>
    /// The body of the request, if applicable.
    /// </summary>
    public string? Request { get; set; }

    /// <summary>
    /// The response body content, if any.
    /// </summary>
    public string? Response { get; set; }

    /// <summary>
    /// The HTTP request headers.
    /// </summary>
    public IDictionary<string, string> RequestHeaders
    {
        get => field ?? new Dictionary<string, string>();
        set => field = value.Where(kvp => _safeHeaders.Contains(kvp.Key)).ToDictionary();
    }

    /// <summary>
    /// The timestamp when the request was received (UTC).
    /// </summary>
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// The status code returned in the response.
    /// </summary>
    public int StatusCode { get; set; }

    /// <summary>
    /// The total duration of the HTTP transaction in milliseconds.
    /// </summary>
    public double DurationMs { get; set; }
}