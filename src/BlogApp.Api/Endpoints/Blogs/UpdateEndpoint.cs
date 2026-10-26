using BlogApp.Api.Endpoints.Blogs.Requests;
using BlogApp.Api.Extensions;
using BlogApp.Application.Blogs.Commands;
using BlogApp.Core.Mediator.Abstractions;
using BlogApp.Core.Results;
using BlogApp.Domain.Models.Blogs;
using Microsoft.AspNetCore.Mvc;

namespace BlogApp.Api.Endpoints.Blogs;

public static class UpdateEndpoint
{
    extension(RouteGroupBuilder builder)
    {
        public RouteGroupBuilder UpdateEndpoint()
        {
            builder.MapPut("{id:int}",
                    async ([FromBody] UpdateBlogRequest request, CancellationToken cancellationToken,
                        [FromServices] IMediator mediator) =>
                    {
                        var command = (UpdateBlogCommand)request;
                        var result =
                            await mediator.Send<UpdateBlogCommand, Result<BlogDetailsResult>>(command,
                                cancellationToken);

                        return result.ToResponse();
                    })
                .RequireAuthorization()
                .Produces<Result<BlogDetailsResult>>()
                .Produces<Result>(StatusCodes.Status403Forbidden)
                .Produces<Result>(StatusCodes.Status404NotFound)
                .WithName("UpdateBlog");

            return builder;
        }
    }
}