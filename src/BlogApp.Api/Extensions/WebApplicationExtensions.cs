using BlogApp.Api.Endpoints;

namespace BlogApp.Api.Extensions;

public static class WebApplicationExtensions
{
    extension(WebApplication app)
    {
        public WebApplication MapEndpoints()
        {
            app.RegisterAuthEndpoints();
            app.RegisterBlogEndpoints();
            app.RegisterCategoryEndpoints();

            return app;
        }
    }
}