using BlogApp.Api.Endpoints.Auth.Requests;
using BlogApp.Api.Extensions;
using BlogApp.Application.Auth.Commands;
using BlogApp.Core.Mediator.Abstractions;
using BlogApp.Core.Results;
using BlogApp.Domain.Models.Auth;
using Microsoft.AspNetCore.Mvc;

namespace BlogApp.Api.Endpoints.Auth;

public static class Verify2FAEndpoint
{
    extension(RouteGroupBuilder builder)
    {
        public RouteGroupBuilder Verify2FAEndpoint()
        {
            builder.MapPost("2fa/verify",
                    async ([FromBody] Verify2FARequest request, CancellationToken cancellationToken,
                        [FromServices] IMediator mediator) =>
                    {
                        var command = new Verify2FACommand(request.Code);
                        var result =
                            await mediator.Send<Verify2FACommand, Result<LoginResult>>(command, cancellationToken);

                        return result.ToResponse();
                    })
                .RequireAuthorization()
                .Produces<Result<LoginResult>>()
                .Produces<Result>(StatusCodes.Status400BadRequest)
                .WithName("Verify2FA");

            return builder;
        }
    }
}