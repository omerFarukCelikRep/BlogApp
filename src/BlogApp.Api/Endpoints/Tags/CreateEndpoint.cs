using BlogApp.Api.Endpoints.Tags.Requests;
using BlogApp.Api.Extensions;
using BlogApp.Application.Tags.Commands;
using BlogApp.Core.Mediator.Abstractions;
using BlogApp.Core.Results;
using BlogApp.Domain.Models.Tags;
using Microsoft.AspNetCore.Mvc;

namespace BlogApp.Api.Endpoints.Tags;

public static class CreateEndpoint
{
    extension(RouteGroupBuilder builder)
    {
        public RouteGroupBuilder CreateEndpoint()
        {
            builder.MapPost(string.Empty,
                    async ([FromBody] CreateTagRequest request, CancellationToken cancellationToken,
                        [FromServices] IMediator mediator) =>
                    {
                        var command = (CreateTagCommand)request;
                        var result =
                            await mediator.Send<CreateTagCommand, Result<TagResult>>(command, cancellationToken);

                        return result.ToResponse();
                    })
                .RequireAuthorization()
                .Produces<Result>()
                .Produces<Result>(StatusCodes.Status400BadRequest)
                .WithName("CreateTag");

            return builder;
        }
    }
}