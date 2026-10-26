using BlogApp.Api.Endpoints.Categories.Requests;
using BlogApp.Api.Endpoints.Tags.Requests;
using BlogApp.Api.Extensions;
using BlogApp.Application.Categories.Queries;
using BlogApp.Core.Mediator.Abstractions;
using BlogApp.Core.Results;
using BlogApp.Domain.Models.Categories;
using Microsoft.AspNetCore.Mvc;

namespace BlogApp.Api.Endpoints.Categories;

public static class GetAllForAdminEndpoint
{
    extension(RouteGroupBuilder builder)
    {
        public RouteGroupBuilder GetAllForAdminEndpoint()
        {
            builder.MapGet("all/admin",
                    async ([AsParameters] GetAllCategoriesForAdminRequest request, CancellationToken cancellationToken,
                        [FromServices] IMediator mediator) =>
                    {
                        var query = new GetAllForAdminQuery(request.Page, request.PageSize);
                        var result =
                            await mediator.Send<GetAllForAdminQuery, PaginatedResult<CategoryResult>>(query,
                                cancellationToken);

                        return result.ToResponse();
                    })
                .RequireAuthorization()
                .Produces<PaginatedResult<CategoryResult>>()
                .WithName("GetAllForAdmin");

            return builder;
        }
    }
}