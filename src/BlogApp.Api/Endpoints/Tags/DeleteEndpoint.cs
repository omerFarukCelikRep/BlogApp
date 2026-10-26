using BlogApp.Api.Extensions;
using BlogApp.Application.Tags.Commands;
using BlogApp.Core.Mediator.Abstractions;
using BlogApp.Core.Results;
using Microsoft.AspNetCore.Mvc;

namespace BlogApp.Api.Endpoints.Tags;

public static class DeleteEndpoint
{
    extension(RouteGroupBuilder builder)
    {
        public RouteGroupBuilder DeleteEndpoint()
        {
            builder.MapDelete("{id:int}",
                    async (int id, CancellationToken cancellationToken,
                        [FromServices] IMediator mediator) =>
                    {
                        var command = new DeleteTagCommand(id);
                        var result =
                            await mediator.Send<DeleteTagCommand, Result>(command, cancellationToken);

                        return result.ToResponse();
                    })
                .RequireAuthorization()
                .Produces<Result>()
                .WithName("DeleteTag");

            return builder;
        }
    }
}