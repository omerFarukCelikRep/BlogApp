namespace BlogApp.Api.Handlers;

public static class ApplicationBuilderExtensions
{
    public static IApplicationBuilder UseCustomExceptionHandler(this IApplicationBuilder app)
    {
        return app.UseExceptionHandler();
    }

    public static IApplicationBuilder UseCorrelation(this IApplicationBuilder app)
    {
        return app.UseMiddleware<CorrelationHandler>();
    }
}