using BlogApp.Api.Endpoints;

namespace BlogApp.Api.Extensions;

public static class WebApplicationExtensions
{
    public static RouteGroupBuilder MapEndpoints(this WebApplication app)
    {
        return app.RegisterAuthEndpoints();
    }
}