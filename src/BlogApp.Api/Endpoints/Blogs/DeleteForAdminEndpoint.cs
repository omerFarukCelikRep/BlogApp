using BlogApp.Api.Extensions;
using BlogApp.Application.Blogs.Commands;
using BlogApp.Core.Mediator.Abstractions;
using BlogApp.Core.Results;
using Microsoft.AspNetCore.Mvc;

namespace BlogApp.Api.Endpoints.Blogs;

public static class DeleteForAdminEndpoint
{
    extension(RouteGroupBuilder builder)
    {
        public RouteGroupBuilder DeleteForAdminEndpoint()
        {
            builder.MapDelete("{id:int}/admin",
                    async (int id, CancellationToken cancellationToken, [FromServices] IMediator mediator) =>
                    {
                        var command = new DeleteBlogForAdminCommand(id);
                        var result = await mediator.Send<DeleteBlogForAdminCommand, Result>(command, cancellationToken);

                        return result.ToResponse();
                    })
                .RequireAuthorization()
                .Produces<Result>()
                .WithName("DeleteBlogForAdmin");

            return builder;
        }
    }
}