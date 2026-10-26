using BlogApp.Api.Extensions;
using BlogApp.Application.Blogs.Queries;
using BlogApp.Core.Mediator.Abstractions;
using BlogApp.Core.Results;
using BlogApp.Domain.Models.Blogs;
using Microsoft.AspNetCore.Mvc;

namespace BlogApp.Api.Endpoints.Blogs;

public static class ForUpdateEndpoint
{
    extension(RouteGroupBuilder builder)
    {
        public RouteGroupBuilder ForUpdateEndpoint()
        {
            builder.MapGet("{id:int}/edit",
                    async (int id, CancellationToken cancellationToken, [FromServices] IMediator mediator) =>
                    {
                        var query = new GetForUpdateQuery(id);
                        var result =
                            await mediator.Send<GetForUpdateQuery, Result<BlogForUpdateResult>>(query,
                                cancellationToken);

                        return result.ToResponse();
                    })
                .RequireAuthorization()
                .Produces<Result<BlogForUpdateResult>>()
                .Produces<Result>(StatusCodes.Status403Forbidden)
                .Produces<Result>(StatusCodes.Status404NotFound)
                .WithName("GetBlogForUpdate");

            return builder;
        }
    }
}