using BlogApp.Api.Extensions;
using BlogApp.Application.Categories.Queries;
using BlogApp.Core.Mediator.Abstractions;
using BlogApp.Core.Results;
using BlogApp.Domain.Models.Blogs;
using BlogApp.Domain.Models.Categories;
using Microsoft.AspNetCore.Mvc;

namespace BlogApp.Api.Endpoints.Categories;

public static class GetBySlugEndpoint
{
    extension(RouteGroupBuilder builder)
    {
        public RouteGroupBuilder GetBySlugEndpoint()
        {
            builder.MapGet("{slug}",
                    async (string slug, CancellationToken cancellationToken, [FromServices] IMediator mediator) =>
                    {
                        var query = new GetBySlugQuery(slug);
                        var result =
                            await mediator.Send<GetBySlugQuery, Result<CategoryDetailsResult>>(query,
                                cancellationToken);

                        return result.ToResponse();
                    })
                .AllowAnonymous()
                .Produces<Result<CategoryDetailsResult>>()
                .Produces<Result>(StatusCodes.Status404NotFound)
                .WithName("GetCategoryBySlug");

            return builder;
        }
    }
}