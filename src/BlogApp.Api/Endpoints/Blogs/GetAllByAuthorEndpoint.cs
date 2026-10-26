using BlogApp.Api.Endpoints.Blogs.Requests;
using BlogApp.Core.Mediator.Abstractions;
using BlogApp.Core.Results;
using BlogApp.Domain.Models.Blogs;
using Microsoft.AspNetCore.Mvc;
using OpenTelemetry.Trace;

namespace BlogApp.Api.Endpoints.Blogs;

public static class GetAllByAuthorEndpoint
{
    extension(RouteGroupBuilder builder)
    {
        public RouteGroupBuilder GetAllByAuthorEndpoint()
        {
            builder.MapGet("{username}/author", async ([AsParameters] GetAllByAuthorRequest request,CancellationToken cancellationToken, [FromServices] IMediator mediator) =>
                {

                })
                .AllowAnonymous()
                .Produces<PaginatedResult<BlogSummaryResult>>()
                .Produces<Result>(StatusCodes.Status404NotFound)
                .WithName("GetAllBlogsByAuthor");
            
            return builder;
        }
    }
}