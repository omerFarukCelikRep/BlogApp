using Asp.Versioning;
using BlogApp.Api.Endpoints.Search;

namespace BlogApp.Api.Endpoints;

public static class SearchEndpoints
{
    public static RouteGroupBuilder RegisterSearchEndpoints(this WebApplication app)
    {
        var apiVersionSet = app.NewApiVersionSet()
            .HasApiVersion(new ApiVersion(1, 0))
            .ReportApiVersions()
            .Build();

        var searchGroup = app.MapGroup("api/v{version:apiVersion}/search")
            .WithApiVersionSet(apiVersionSet)
            .WithTags("Search")
            .SearchEndpoint()
            .ReindexEndpoint();

        return searchGroup;
    }
}