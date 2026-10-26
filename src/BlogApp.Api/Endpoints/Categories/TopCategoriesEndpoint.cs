using BlogApp.Api.Endpoints.Categories.Requests;
using BlogApp.Api.Endpoints.Shared.Responses;
using BlogApp.Api.Extensions;
using BlogApp.Application.Categories.Queries;
using BlogApp.Core.Mediator.Abstractions;
using BlogApp.Core.Results;
using BlogApp.Domain.Models.Categories;
using Microsoft.AspNetCore.Mvc;

namespace BlogApp.Api.Endpoints.Categories;

public static class TopCategoriesEndpoint
{
    extension(RouteGroupBuilder builder)
    {
        public RouteGroupBuilder TopCategoriesEndpoint()
        {
            builder.MapGet("top", async ([AsParameters] TopCategoryRequest request,CancellationToken cancellationToken, [FromServices] IMediator mediator) =>
                {
                    var query = (GetTopCategoriesQuery)request;
                    var result =
                        await mediator.Send<GetTopCategoriesQuery, Result<List<CategoryResult>>>(query,
                            cancellationToken);
                    
                    return result.ToResponse();
                })
            .AllowAnonymous()
            .Produces<Response<List<CategoryResult>>>()
            .WithName("Top Categories")
            .WithTags("TopCategories");

            return builder;
        }
    }
}