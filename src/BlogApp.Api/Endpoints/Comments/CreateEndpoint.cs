using BlogApp.Api.Endpoints.Comments.Requests;
using BlogApp.Api.Extensions;
using BlogApp.Api.Filters;
using BlogApp.Application.Comments.Commands;
using BlogApp.Core.Mediator.Abstractions;
using BlogApp.Core.Results;
using BlogApp.Domain.Models.Comments;
using Microsoft.AspNetCore.Mvc;

namespace BlogApp.Api.Endpoints.Comments;

public static class CreateEndpoint
{
    extension(RouteGroupBuilder builder)
    {
        public RouteGroupBuilder CreateEndpoint()
        {
            builder.MapPost("/",
                    async ([FromBody] CreateCommentRequest request, CancellationToken cancellationToken,
                        [FromServices] IMediator mediator) =>
                    {
                        var command = (CreateCommentCommand)request;
                        var result =
                            await mediator.Send<CreateCommentCommand, Result<CommentResult>>(command,
                                cancellationToken);

                        return result.ToResponse();
                    })
                .RequireAuthorization()
                .Produces<Result<CommentResult>>()
                .Produces<Result>(StatusCodes.Status400BadRequest)
                .Produces<Result>(StatusCodes.Status401Unauthorized)
                .AddEndpointFilter<XssProtectionEndpointFilter>()
                .WithName("CreateComment");

            return builder;
        }
    }
}