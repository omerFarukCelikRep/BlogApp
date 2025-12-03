using BlogApp.Api.Endpoints;

namespace BlogApp.Api.Extensions;

public static class WebApplicationExtensions
{
    extension(WebApplication app)
    {
        public RouteGroupBuilder MapEndpoints()
        {
            return app.RegisterAuthEndpoints();
        }
    }
}