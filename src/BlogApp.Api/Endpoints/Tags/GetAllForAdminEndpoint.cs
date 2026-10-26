using BlogApp.Api.Endpoints.Tags.Requests;
using BlogApp.Api.Extensions;
using BlogApp.Application.Tags.Queries;
using BlogApp.Core.Mediator.Abstractions;
using BlogApp.Core.Results;
using BlogApp.Domain.Models.Tags;
using Microsoft.AspNetCore.Mvc;

namespace BlogApp.Api.Endpoints.Tags;

public static class GetAllForAdminEndpoint
{
    extension(RouteGroupBuilder builder)
    {
        public RouteGroupBuilder GetAllForAdminEndpoint()
        {
            builder.MapGet("all/admin",
                    async ([AsParameters] GetAllTagsForAdminRequest request, CancellationToken cancellationToken,
                        [FromServices] IMediator mediator) =>
                    {
                        var query = new GetAllForAdminQuery(request.Page, request.PageSize);
                        var result =
                            await mediator.Send<GetAllForAdminQuery, PaginatedResult<TagResult>>(query,
                                cancellationToken);

                        return result.ToResponse();
                    })
                .RequireAuthorization()
                .Produces<PaginatedResult<TagResult>>()
                .WithName("GetAllForAdmin");

            return builder;
        }
    }
}