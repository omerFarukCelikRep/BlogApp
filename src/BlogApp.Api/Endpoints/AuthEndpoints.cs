using BlogApp.Api.Endpoints.Auth;

namespace BlogApp.Api.Endpoints;

public static class AuthEndpoints
{
    public static RouteGroupBuilder RegisterAuthEndpoints(this WebApplication app)
    {
        var authGroup = app.MapGroup("auth");
        authGroup.LoginEndpoints();

        return authGroup;
    }
}