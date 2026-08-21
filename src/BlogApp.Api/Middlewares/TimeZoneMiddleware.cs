namespace BlogApp.Api.Middlewares;

public class TimeZoneMiddleware(RequestDelegate next)
{
    private const string TimeZoneHeaderName = "X-TimeZone";

    public async Task InvokeAsync(HttpContext context)
    {
        var timeZoneId = context.Request.Headers[TimeZoneHeaderName].FirstOrDefault();

        TimeZoneInfo timeZoneInfo;
        try
        {
            timeZoneInfo = string.IsNullOrEmpty(timeZoneId)
                ? TimeZoneInfo.Utc
                : TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);
        }
        catch
        {
            timeZoneInfo = TimeZoneInfo.Utc;
        }

        context.Items["TimeZone"] = timeZoneInfo;
        await next(context);
    }
}