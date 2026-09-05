using BlogApp.Api.Endpoints.Blogs.Requests;
using BlogApp.Api.Endpoints.Shared.Responses;
using BlogApp.Api.Extensions;
using BlogApp.Application.Blogs.Queries;
using BlogApp.Core.Mediator.Abstractions;
using BlogApp.Core.Results;
using BlogApp.Domain.Models.Blogs;
using Microsoft.AspNetCore.Mvc;

namespace BlogApp.Api.Endpoints.Blogs;

public static class RandomEndpoint
{
    extension(RouteGroupBuilder builder)
    {
        public RouteGroupBuilder RandomEndpoint()
        {
            builder.MapGet("random",
                    async ([AsParameters] RandomBlogRequest request, CancellationToken cancellationToken,
                        [FromServices] IMediator mediator) =>
                    {
                        var query = (GetRandomBlogsQuery)request;
                        var result =
                            await mediator.Send<GetRandomBlogsQuery, Result<List<BlogSummaryResult>>>(query,
                                cancellationToken);

                        return result.ToResponse();
                    })
                .AllowAnonymous()
                .Produces<Response<List<BlogSummaryResult>>>()
                .WithName("Random")
                .WithTags("Random");

            return builder;
        }
    }
}