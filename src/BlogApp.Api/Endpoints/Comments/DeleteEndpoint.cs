using BlogApp.Api.Endpoints.Comments.Requests;
using BlogApp.Api.Extensions;
using BlogApp.Application.Comments.Commands;
using BlogApp.Core.Mediator.Abstractions;
using BlogApp.Core.Results;
using Microsoft.AspNetCore.Mvc;

namespace BlogApp.Api.Endpoints.Comments;

public static class DeleteEndpoint
{
    extension(RouteGroupBuilder builder)
    {
        public RouteGroupBuilder DeleteEndpoint()
        {
            builder.MapDelete("{blogId:int}/{id:int}",
                    async ([AsParameters] DeleteCommentRequest request, CancellationToken cancellationToken,
                        [FromServices] IMediator mediator) =>
                    {
                        var command = (DeleteCommentCommand)request;
                        var result = await mediator.Send<DeleteCommentCommand, Result>(command, cancellationToken);

                        return result.ToResponse();
                    })
                .RequireAuthorization()
                .Produces<Result>()
                .Produces<Result>(StatusCodes.Status403Forbidden)
                .Produces<Result>(StatusCodes.Status404NotFound)
                .WithName("DeleteComment");

            return builder;
        }
    }
}