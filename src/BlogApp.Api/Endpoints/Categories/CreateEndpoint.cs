using BlogApp.Api.Endpoints.Categories.Requests;
using BlogApp.Api.Extensions;
using BlogApp.Application.Categories.Commands;
using BlogApp.Core.Mediator.Abstractions;
using BlogApp.Core.Results;
using BlogApp.Domain.Models.Categories;
using Microsoft.AspNetCore.Mvc;

namespace BlogApp.Api.Endpoints.Categories;

public static class CreateEndpoint
{
    extension(RouteGroupBuilder builder)
    {
        public RouteGroupBuilder CreateEndpoint()
        {
            builder.MapPost(string.Empty,
                    async ([FromBody] CreateCategoryRequest request, CancellationToken cancellationToken,
                        [FromServices] IMediator mediator) =>
                    {
                        var command = (CreateCategoryCommand)request;
                        var result =
                            await mediator.Send<CreateCategoryCommand, Result<CategoryResult>>(command,
                                cancellationToken);

                        return result.ToResponse();
                    })
                .RequireAuthorization()
                .Produces<Result>()
                .Produces<Result>(StatusCodes.Status400BadRequest)
                .WithName("CreateCategory");

            return builder;
        }
    }
}