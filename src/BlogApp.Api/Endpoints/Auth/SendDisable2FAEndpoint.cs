using BlogApp.Api.Extensions;
using BlogApp.Application.Auth.Commands;
using BlogApp.Core.Mediator.Abstractions;
using BlogApp.Core.Results;
using Microsoft.AspNetCore.Mvc;

namespace BlogApp.Api.Endpoints.Auth;

public static class SendDisable2FAEndpoint
{
    extension(RouteGroupBuilder builder)
    {
        public RouteGroupBuilder SendDisable2FAEndpoint()
        {
            builder.MapPost("2fa/disable/confirm",
                    async (CancellationToken cancellationToken, [FromServices] IMediator mediator) =>
                    {
                        var command = new SendDisable2FACommand();
                        var result = await mediator.Send<SendDisable2FACommand, Result>(command, cancellationToken);

                        return result.ToResponse();
                    })
                .RequireAuthorization()
                .Produces<Result>()
                .WithName("SendDisable2FA");

            return builder;
        }
    }
}