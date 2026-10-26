using BlogApp.Api.Endpoints.Blogs.Requests;
using BlogApp.Api.Extensions;
using BlogApp.Application.Blogs.Queries;
using BlogApp.Core.Mediator.Abstractions;
using BlogApp.Core.Results;
using BlogApp.Domain.Models.Blogs;
using Microsoft.AspNetCore.Mvc;

namespace BlogApp.Api.Endpoints.Blogs;

public static class GetAllByTagEndpoint
{
    extension(RouteGroupBuilder builder)
    {
        public RouteGroupBuilder GetAllByTagEndpoint()
        {
            builder.MapGet("{slug}/tag",
                    async ([AsParameters] GetAllByTagRequest request, CancellationToken cancellationToken,
                        [FromServices] IMediator mediator) =>
                    {
                        var query = (GetAllByTagQuery)request;
                        var result =
                            await mediator.Send<GetAllByTagQuery, PaginatedResult<BlogSummaryResult>>(query,
                                cancellationToken);

                        return result.ToResponse();
                    })
                .AllowAnonymous()
                .Produces<PaginatedResult<BlogSummaryResult>>()
                .Produces<Result>(StatusCodes.Status404NotFound)
                .WithName("GetBlogsByTag");

            return builder;
        }
    }
}