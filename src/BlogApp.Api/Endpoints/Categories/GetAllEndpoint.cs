using BlogApp.Api.Extensions;
using BlogApp.Application.Categories.Queries;
using BlogApp.Core.Mediator.Abstractions;
using BlogApp.Core.Results;
using BlogApp.Domain.Models.Blogs;
using BlogApp.Domain.Models.Categories;
using Microsoft.AspNetCore.Mvc;

namespace BlogApp.Api.Endpoints.Categories;

public static class GetAllEndpoint
{
    extension(RouteGroupBuilder builder)
    {
        public RouteGroupBuilder GetAllEndpoint()
        {
            builder.MapGet(string.Empty,
                    async (CancellationToken cancellationToken, [FromServices] IMediator mediator) =>
                    {
                        var result =
                            await mediator.Send<GetAllQuery, Result<List<CategoryDetailsResult>>>(new GetAllQuery(),
                                cancellationToken);

                        return result.ToResponse();
                    })
                .AllowAnonymous()
                .Produces<Result<List<BlogDetailsResult>>>()
                .Produces<Result>(StatusCodes.Status404NotFound)
                .WithName("GetAllCategories");

            return builder;
        }
    }
}