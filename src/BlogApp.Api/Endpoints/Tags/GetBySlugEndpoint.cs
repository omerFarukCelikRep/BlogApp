using BlogApp.Api.Extensions;
using BlogApp.Application.Tags.Queries;
using BlogApp.Core.Mediator.Abstractions;
using BlogApp.Core.Results;
using BlogApp.Domain.Models.Tags;
using Microsoft.AspNetCore.Mvc;

namespace BlogApp.Api.Endpoints.Tags;

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
                        var result = await mediator.Send<GetBySlugQuery, Result<TagResult>>(query, cancellationToken);

                        return result.ToResponse();
                    })
                .AllowAnonymous()
                .Produces<Result<TagResult>>()
                .Produces<Result>(StatusCodes.Status404NotFound)
                .WithName("GetTagBySlugEndpoint");

            return builder;
        }
    }
}