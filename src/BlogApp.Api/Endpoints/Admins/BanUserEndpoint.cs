using BlogApp.Api.Endpoints.Admins.Requests;
using BlogApp.Api.Extensions;
using BlogApp.Application.Admins.Commands;
using BlogApp.Core.Mediator.Abstractions;
using BlogApp.Core.Results;
using Microsoft.AspNetCore.Mvc;

namespace BlogApp.Api.Endpoints.Admins;

public static class BanUserEndpoint
{
    extension(RouteGroupBuilder builder)
    {
        public RouteGroupBuilder BanUserEndpoint()
        {
            builder.MapPut("{id:guid}/ban",
                    async ([FromBody] BanUserRequest request, CancellationToken cancellationToken,
                        [FromServices] IMediator mediator) =>
                    {
                        var command = (BanUserCommand)request;
                        var result = await mediator.Send<BanUserCommand, Result>(command, cancellationToken);

                        return result.ToResponse();
                    })
                .RequireAuthorization()
                .Produces<Result>()
                .Produces<Result>(StatusCodes.Status400BadRequest)
                .WithName("BanUser");

            return builder;
        }
    }
}