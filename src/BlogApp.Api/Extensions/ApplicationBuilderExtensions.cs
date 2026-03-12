using BlogApp.Api.Middlewares;

namespace BlogApp.Api.Extensions;

public static class ApplicationBuilderExtensions
{
    extension(IApplicationBuilder app)
    {
        public IApplicationBuilder UseCustomExceptionHandler()
        {
            return app.UseExceptionHandler();
        }

        public IApplicationBuilder UseCorrelation()
        {
            return app.UseMiddleware<CorrelationMiddleware>();
        }
    }
}