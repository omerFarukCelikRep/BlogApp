using BlogApp.Api.Extensions;
using BlogApp.Application.Comments.Queries;
using BlogApp.Core.Mediator.Abstractions;
using BlogApp.Core.Results;
using BlogApp.Domain.Models.Comments;
using Microsoft.AspNetCore.Mvc;

namespace BlogApp.Api.Endpoints.Comments;

public static class GetAllByBlogEndpoint
{
    extension(RouteGroupBuilder builder)
    {
        public RouteGroupBuilder GetAllByBlogEndpoint()
        {
            builder.MapGet("{blogId:int}",
                    async (int blogId, CancellationToken cancellationToken, [FromServices] IMediator mediator) =>
                    {
                        var query = new GetBlogCommentsQuery(blogId);
                        var result =
                            await mediator.Send<GetBlogCommentsQuery, Result<List<CommentResult>>>(query,
                                cancellationToken);

                        return result.ToResponse();
                    })
                .AllowAnonymous()
                .Produces<List<CommentResult>>()
                .WithName("BlogComments");

            return builder;
        }
    }
}