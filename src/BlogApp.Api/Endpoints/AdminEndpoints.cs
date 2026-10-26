using Asp.Versioning;
using BlogApp.Api.Endpoints.Admins;
using BlogApp.Api.Endpoints.Authors;

namespace BlogApp.Api.Endpoints;

public static class AdminEndpoints
{
    public static RouteGroupBuilder RegisterAdminEndpoints(this WebApplication app)
    {
        var apiVersionSet = app.NewApiVersionSet()
            .HasApiVersion(new ApiVersion(1, 0))
            .ReportApiVersions()
            .Build();

        var authorGroup = app.MapGroup("api/v{version:apiVersion}/admins")
            .WithApiVersionSet(apiVersionSet)
            .WithTags("Admins")
            .DashboardEndpoints()
            .UserAdminEndpoint()
            .BanUserEndpoint()
            .AssignRoleEndpoint();

        return authorGroup;
    }
}