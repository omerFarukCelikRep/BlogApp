using BlogApp.Api.Endpoints.Shared.Responses;
using BlogApp.Api.Extensions;
using BlogApp.Application.Blogs.Queries;
using BlogApp.Core.Mediator.Abstractions;
using BlogApp.Core.Results;
using BlogApp.Domain.Models.Blogs;
using Microsoft.AspNetCore.Mvc;

namespace BlogApp.Api.Endpoints.Blogs;

public static class DetailsEndpoint
{
    extension(RouteGroupBuilder builder)
    {
        public RouteGroupBuilder DetailsEndpoint()
        {
            builder.MapGet("{id:int}",
                    async (int id, CancellationToken cancellationToken, [FromServices] IMediator mediator) =>
                    {
                        var query = new GetByIdQuery(id);
                        var result =
                            await mediator.Send<GetByIdQuery, Result<BlogDetailsResult>>(query, cancellationToken);

                        return result.ToResponse();
                    })
                .AllowAnonymous()
                .Produces<Response<Result<BlogDetailsResult>>>()
                .Produces<Response<Result>>()
                .WithName("Details")
                .WithTags("Details");

            return builder;
        }
    }
}