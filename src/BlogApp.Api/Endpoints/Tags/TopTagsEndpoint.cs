using BlogApp.Api.Endpoints.Shared.Responses;
using BlogApp.Api.Endpoints.Tags.Requests;
using BlogApp.Api.Extensions;
using BlogApp.Application.Tags.Queries;
using BlogApp.Core.Mediator.Abstractions;
using BlogApp.Core.Results;
using BlogApp.Domain.Models.Tags;
using Microsoft.AspNetCore.Mvc;

namespace BlogApp.Api.Endpoints.Tags;

public static class TopTagsEndpoint
{
    extension(RouteGroupBuilder builder)
    {
        public RouteGroupBuilder TopTagsEndpoint()
        {
            builder.MapGet("top",
                    async ([AsParameters] TopTagsRequest request, CancellationToken cancellationToken,
                        [FromServices] IMediator mediator) =>
                    {
                        var query = (GetTopTagsQuery)request;
                        var result =
                            await mediator.Send<GetTopTagsQuery, Result<List<TagResult>>>(query, cancellationToken);

                        return result.ToResponse();
                    })
                .AllowAnonymous()
                .Produces<Response<List<TagResult>>>()
                .WithName("Top Tags")
                .WithTags("TopTags");

            return builder;
        }
    }
}