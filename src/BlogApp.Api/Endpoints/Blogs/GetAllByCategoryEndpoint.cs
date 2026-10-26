using BlogApp.Api.Endpoints.Blogs.Requests;
using BlogApp.Api.Extensions;
using BlogApp.Application.Blogs.Queries;
using BlogApp.Core.Mediator.Abstractions;
using BlogApp.Core.Results;
using BlogApp.Domain.Models.Blogs;
using Microsoft.AspNetCore.Mvc;

namespace BlogApp.Api.Endpoints.Blogs;

public static class GetAllByCategoryEndpoint
{
    extension(RouteGroupBuilder builder)
    {
        public RouteGroupBuilder GetAllByCategoryEndpoint()
        {
            builder.MapGet("{slug}/category",
                    async ([AsParameters] GetAllByCategoryRequest request, CancellationToken cancellationToken,
                        [FromServices] IMediator mediator) =>
                    {
                        var query = (GetAllByCategoryQuery)request;
                        var result =
                            await mediator.Send<GetAllByCategoryQuery, PaginatedResult<BlogSummaryResult>>(query,
                                cancellationToken);

                        return result.ToResponse();
                    })
                .AllowAnonymous()
                .Produces<PaginatedResult<BlogSummaryResult>>()
                .Produces<Result>(StatusCodes.Status404NotFound)
                .WithName("GetBlogsByCategory");

            return builder;
        }
    }
}