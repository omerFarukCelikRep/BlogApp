using Asp.Versioning;
using BlogApp.Api.Endpoints.Comments;

namespace BlogApp.Api.Endpoints;

public static class CommentEndpoints
{
    public static RouteGroupBuilder RegisterCommentEndpoints(this WebApplication app)
    {
        var apiVersionSet = app.NewApiVersionSet()
            .HasApiVersion(new ApiVersion(1, 0))
            .ReportApiVersions()
            .Build();

        var commentGroup = app.MapGroup("api/v{version:apiVersion}/comments")
            .WithApiVersionSet(apiVersionSet)
            .WithTags("Comments")
            .GetAllByBlogEndpoint()
            .CreateEndpoint();

        return commentGroup;
    }
}