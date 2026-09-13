using BlogApp.Api.Endpoints.Search.Requests;
using BlogApp.Api.Extensions;
using BlogApp.Application.Search;
using BlogApp.Core.Mediator.Abstractions;
using BlogApp.Core.Results;
using BlogApp.Core.Search.Models;
using Microsoft.AspNetCore.Mvc;

namespace BlogApp.Api.Endpoints.Search;

public static class SearchEndpoint
{
    extension(RouteGroupBuilder builder)
    {
        public RouteGroupBuilder SearchEndpoint()
        {
            builder.MapGet(string.Empty,
                    async ([AsParameters] SearchRequest request, CancellationToken cancellationToken,
                        [FromServices] IMediator mediator) =>
                    {
                        var query = (SearchQuery)request;
                        var result = await mediator.Send<SearchQuery, Result<SearchResult>>(query, cancellationToken);

                        return result.ToResponse();
                    })
                .AllowAnonymous()
                .Produces<Result<SearchResult>>()
                .WithName("Search");

            return builder;
        }
    }
}