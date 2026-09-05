using BlogApp.Api.Endpoints.Blogs.Requests;
using BlogApp.Api.Endpoints.Shared.Responses;
using BlogApp.Api.Extensions;
using BlogApp.Application.Blogs.Queries;
using BlogApp.Core.Mediator.Abstractions;
using BlogApp.Core.Results;
using BlogApp.Domain.Models.Blogs;
using Microsoft.AspNetCore.Mvc;

namespace BlogApp.Api.Endpoints.Blogs;

public static class TopReadEndpoint
{
    extension(RouteGroupBuilder builder)
    {
        public RouteGroupBuilder TopReadEndpoint()
        {
            builder.MapGet("top-read",
                    async ([AsParameters] TopReadBlogRequest request, CancellationToken cancellationToken,
                        [FromServices] IMediator mediator) =>
                    {
                        var query = (GetTopReadBlogsQuery)request;
                        var result =
                            await mediator.Send<GetTopReadBlogsQuery, Result<List<BlogSummaryResult>>>(query,
                                cancellationToken);

                        return result.ToResponse();
                    })
                .AllowAnonymous()
                .Produces<Response<List<BlogSummaryResult>>>()
                .WithName("Top Read")
                .WithTags("TopRead");

            return builder;
        }
    }
}