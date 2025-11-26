using BlogApp.Api.Handlers;

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
            return app.UseMiddleware<CorrelationHandler>();
        }
    }
}