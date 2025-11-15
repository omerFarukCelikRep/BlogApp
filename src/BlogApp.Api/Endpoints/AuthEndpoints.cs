using Asp.Versioning;
using Asp.Versioning.Builder;
using BlogApp.Api.Endpoints.Auth;

namespace BlogApp.Api.Endpoints;

public static class AuthEndpoints
{
    public static RouteGroupBuilder RegisterAuthEndpoints(this WebApplication app)
    {
        var apiVersionSet = app.NewApiVersionSet()
            .HasApiVersion(new ApiVersion(1, 0))
            .ReportApiVersions()
            .Build();

        var authGroup = app.MapGroup("api/v{version:apiVersion}/auth")
            .WithApiVersionSet(apiVersionSet);
        authGroup.RegisterEndpoints();
        authGroup.LoginEndpoints();

        return authGroup;
    }
}