using Asp.Versioning;
using BlogApp.Api.Endpoints.Tags;

namespace BlogApp.Api.Endpoints;

public static class TagEndpoints
{
    public static RouteGroupBuilder RegisterTagEndpoints(this WebApplication app)
    {
        var apiVersionSet = app.NewApiVersionSet()
            .HasApiVersion(new ApiVersion(1, 0))
            .ReportApiVersions()
            .Build();

        var tagGroup = app.MapGroup("api/v{version:apiVersion}/tags")
            .WithApiVersionSet(apiVersionSet)
            .WithTags("Tags")
            .TopTagsEndpoint();

        return tagGroup;
    }
}