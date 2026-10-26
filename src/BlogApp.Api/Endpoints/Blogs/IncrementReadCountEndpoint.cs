using BlogApp.Api.Endpoints.Shared.Responses;
using BlogApp.Api.Extensions;
using BlogApp.Application.Blogs.Commands;
using BlogApp.Core.Mediator.Abstractions;
using BlogApp.Core.Results;
using Microsoft.AspNetCore.Mvc;

namespace BlogApp.Api.Endpoints.Blogs;

public static class IncrementReadCountEndpoint
{
    extension(RouteGroupBuilder builder)
    {
        public RouteGroupBuilder IncrementReadCountEndpoint()
        {
            builder.MapPost("{id:int}/read",
                    async (int id, CancellationToken cancellationToken, [FromServices] IMediator mediator) =>
                    {
                        var command = new IncrementReadCountCommand(id);
                        var result = await mediator.Send<IncrementReadCountCommand, Result>(command, cancellationToken);

                        return result.ToResponse();
                    })
                .AllowAnonymous()
                .Produces<Response<Result>>()
                .WithName("IncrementReadCount")
                .WithTags("IncrementReadCount");

            return builder;
        }
    }
}