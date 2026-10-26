using BlogApp.Api.Endpoints.Blogs.Requests;
using BlogApp.Api.Extensions;
using BlogApp.Application.Blogs.Queries;
using BlogApp.Core.Mediator.Abstractions;
using BlogApp.Core.Results;
using BlogApp.Domain.Models.Blogs;
using Microsoft.AspNetCore.Mvc;

namespace BlogApp.Api.Endpoints.Blogs;

public static class GetAllForAdminEndpoint
{
    extension(RouteGroupBuilder builder)
    {
        public RouteGroupBuilder GetAllForAdminEndpoint()
        {
            builder.MapGet("all/admin",
                    async ([AsParameters] GetAllForAdminRequest request, CancellationToken cancellationToken,
                        [FromServices] IMediator mediator) =>
                    {
                        var query = (GetAllForAdminQuery)request;
                        var result =
                            await mediator.Send<GetAllForAdminQuery, PaginatedResult<AdminBlogSummaryResult>>(query,
                                cancellationToken);

                        return result.ToResponse();
                    })
                .RequireAuthorization()
                .Produces<PaginatedResult<AdminBlogSummaryResult>>()
                .WithName("GetAllForAdmin");

            return builder;
        }
    }
}