using BlogApp.Api.Endpoints.Comments.Requests;
using BlogApp.Api.Extensions;
using BlogApp.Application.Comments.Queries;
using BlogApp.Core.Mediator.Abstractions;
using BlogApp.Core.Results;
using BlogApp.Domain.Models.Comments;
using Microsoft.AspNetCore.Mvc;

namespace BlogApp.Api.Endpoints.Comments;

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
                            await mediator.Send<GetAllForAdminQuery, PaginatedResult<CommentResult>>(query,
                                cancellationToken);

                        return result.ToResponse();
                    })
                .RequireAuthorization()
                .Produces<PaginatedResult<CommentResult>>()
                .WithName("GetAllForAdmin");

            return builder;
        }
    }
}