using BlogApp.Api.Extensions;
using BlogApp.Application.Categories.Commands;
using BlogApp.Core.Mediator.Abstractions;
using BlogApp.Core.Results;
using Microsoft.AspNetCore.Mvc;

namespace BlogApp.Api.Endpoints.Categories;

public static class DeleteEndpoint
{
    extension(RouteGroupBuilder builder)
    {
        public RouteGroupBuilder DeleteEndpoint()
        {
            builder.MapDelete("{id:int}",
                    async (int id, CancellationToken cancellationToken,
                        [FromServices] IMediator mediator) =>
                    {
                        var command = new DeleteCategoryCommand(id);
                        var result =
                            await mediator.Send<DeleteCategoryCommand, Result>(command, cancellationToken);

                        return result.ToResponse();
                    })
                .RequireAuthorization()
                .Produces<Result>()
                .WithName("DeleteCategory");

            return builder;
        }
    }
}