using Asp.Versioning;
using BlogApp.Api.Endpoints.Authors;

namespace BlogApp.Api.Endpoints;

public static class AuthorEndpoints
{
    public static RouteGroupBuilder RegisterAuthorEndpoints(this WebApplication app)
    {
        var apiVersionSet = app.NewApiVersionSet()
            .HasApiVersion(new ApiVersion(1, 0))
            .ReportApiVersions()
            .Build();

        var authorGroup = app.MapGroup("api/v{version:apiVersion}/authors")
            .WithApiVersionSet(apiVersionSet)
            .WithTags("Authors")
            .GetByUsernameEndpoint();

        return authorGroup;
    }
}