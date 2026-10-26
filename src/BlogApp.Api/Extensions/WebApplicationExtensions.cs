using BlogApp.Api.Endpoints;

namespace BlogApp.Api.Extensions;

public static class WebApplicationExtensions
{
    extension(WebApplication app)
    {
        public WebApplication MapEndpoints()
        {
            app.RegisterAdminEndpoints();
            app.RegisterAuthEndpoints();
            app.RegisterAuthorEndpoints();
            app.RegisterBlogEndpoints();
            app.RegisterCategoryEndpoints();
            app.RegisterCommentEndpoints();
            app.RegisterSearchEndpoints();
            app.RegisterTagEndpoints();

            return app;
        }
    }
}