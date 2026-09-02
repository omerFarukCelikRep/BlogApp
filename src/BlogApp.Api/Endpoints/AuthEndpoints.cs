using Asp.Versioning;
using BlogApp.Api.Endpoints.Auth;

namespace BlogApp.Api.Endpoints;

public static class AuthEndpoints
{
    extension(WebApplication app)
    {
        public RouteGroupBuilder RegisterAuthEndpoints()
        {
            var apiVersionSet = app.NewApiVersionSet()
                .HasApiVersion(new ApiVersion(1, 0))
                .ReportApiVersions()
                .Build();

            var authGroup = app.MapGroup("api/v{version:apiVersion}/auth")
                .WithApiVersionSet(apiVersionSet)
                .WithTags("Auth")
                .RegisterEndpoint()
                .LoginEndpoint()
                .LogoutEndpoint()
                .RefreshTokenEndpoint();

            return authGroup;
        }
    }
}