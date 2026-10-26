using Asp.Versioning;
using BlogApp.Api.Endpoints.Blogs;

namespace BlogApp.Api.Endpoints;

public static class BlogEndpoints
{
    public static RouteGroupBuilder RegisterBlogEndpoints(this WebApplication app)
    {
        var apiVersionSet = app.NewApiVersionSet()
            .HasApiVersion(new ApiVersion(1, 0))
            .ReportApiVersions()
            .Build();

        var blogGroup = app.MapGroup("api/v{version:apiVersion}/blogs")
            .WithApiVersionSet(apiVersionSet)
            .WithTags("Blogs")
            .TopReadEndpoint()
            .RandomEndpoint()
            .CreateEndpoint()
            .DetailsEndpoint()
            .IncrementReadCountEndpoint()
            .ToggleLikeEndpoint()
            .ForUpdateEndpoint()
            .UpdateEndpoint()
            .GetAllByTagEndpoint()
            .GetAllByCategoryEndpoint()
            .GetAllByAuthorEndpoint()
            .GetAllForAdminEndpoint();

        return blogGroup;
    }
}