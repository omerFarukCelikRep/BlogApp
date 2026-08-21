namespace BlogApp.Api.Extensions;

public static class HttpContextExtensions
{
    extension(HttpContext context)
    {
        public TimeZoneInfo? GetUserTimeZone()
        {
            return (TimeZoneInfo?)context.Items["TimeZone"];
        }
    }
}