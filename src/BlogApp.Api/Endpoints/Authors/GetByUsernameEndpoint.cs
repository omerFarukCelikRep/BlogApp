using BlogApp.Api.Extensions;
using BlogApp.Application.Authors;
using BlogApp.Core.Mediator.Abstractions;
using BlogApp.Core.Results;
using BlogApp.Domain.Models.Authors;
using Microsoft.AspNetCore.Mvc;

namespace BlogApp.Api.Endpoints.Authors;

public static class GetByUsernameEndpoint
{
    extension(RouteGroupBuilder builder)
    {
        public RouteGroupBuilder GetByUsernameEndpoint()
        {
            builder.MapGet("{username}",
                    async (string username, CancellationToken cancellationToken, [FromServices] IMediator mediator) =>
                    {
                        var query = new GetByUsernameQuery(username);
                        var result =
                            await mediator.Send<GetByUsernameQuery, Result<AuthorProfileResult>>(query,
                                cancellationToken);

                        return result.ToResponse();
                    })
                .AllowAnonymous()
                .Produces<Result<AuthorProfileResult>>()
                .Produces<Result>(StatusCodes.Status404NotFound)
                .WithName("GetAuthorByUsername");

            return builder;
        }
    }
}