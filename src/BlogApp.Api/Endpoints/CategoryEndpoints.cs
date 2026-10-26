using Asp.Versioning;
using BlogApp.Api.Endpoints.Categories;

namespace BlogApp.Api.Endpoints;

public static class CategoryEndpoints
{
    public static RouteGroupBuilder RegisterCategoryEndpoints(this WebApplication app)
    {
        var apiVersionSet = app.NewApiVersionSet()
            .HasApiVersion(new ApiVersion(1, 0))
            .ReportApiVersions()
            .Build();

        var categoryGroup = app.MapGroup("api/v{version:apiVersion}/categories")
            .WithApiVersionSet(apiVersionSet)
            .WithTags("Categories")
            .TopCategoriesEndpoint()
            .GetAllEndpoint()
            .GetBySlugEndpoint()
            .GetAllForAdminEndpoint()
            .CreateEndpoint()
            .DeleteEndpoint();

        return categoryGroup;
    }
}