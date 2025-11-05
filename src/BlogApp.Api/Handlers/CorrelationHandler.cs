using BlogApp.Core.Logging.Contexts;

namespace BlogApp.Api.Handlers;

public class CorrelationHandler(RequestDelegate next)
{
    private const string CorrelationHeaderName = "X-Correlation-Id";

    public async Task InvokeAsync(HttpContext context)
    {
        var existingId = context.Request.Headers[CorrelationHeaderName].FirstOrDefault();
        var correlationId = string.IsNullOrWhiteSpace(existingId) ? Guid.CreateVersion7().ToString() : existingId;

        CorrelationContext.Set(correlationId);
        context.Response.Headers[CorrelationHeaderName] = correlationId;

        try
        {
            await next(context);
        }
        finally
        {
            CorrelationContext.Clear();
        }
    }
}