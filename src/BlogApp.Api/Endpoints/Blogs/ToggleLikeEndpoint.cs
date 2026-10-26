using BlogApp.Api.Endpoints.Shared.Responses;
using BlogApp.Api.Extensions;
using BlogApp.Application.Blogs.Commands;
using BlogApp.Core.Mediator.Abstractions;
using BlogApp.Core.Results;
using BlogApp.Domain.Models.Blogs;
using Microsoft.AspNetCore.Mvc;

namespace BlogApp.Api.Endpoints.Blogs;

public static class ToggleLikeEndpoint
{
    extension(RouteGroupBuilder builder)
    {
        public RouteGroupBuilder ToggleLikeEndpoint()
        {
            builder.MapPost("{id:int}/like",
                    async (int id, CancellationToken cancellationToken, [FromServices] IMediator mediator) =>
                    {
                        var command = new ToggleLikeCommand(id);
                        var result =
                            await mediator.Send<ToggleLikeCommand, Result<ToggleLikeResult>>(command,
                                cancellationToken);

                        return result.ToResponse();
                    })
                .RequireAuthorization()
                .Produces<Response<Result<ToggleLikeResult>>>()
                .WithName("ToggleLike")
                .WithTags("ToggleLike");

            return builder;
        }
    }
}