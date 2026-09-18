using BlogApp.Api.Endpoints.Auth.Requests;
using BlogApp.Api.Extensions;
using BlogApp.Application.Auth.Commands;
using BlogApp.Core.Mediator.Abstractions;
using BlogApp.Core.Results;
using Microsoft.AspNetCore.Mvc;

namespace BlogApp.Api.Endpoints.Auth;

public static class ConfirmDisable2FAEndpoint
{
    extension(RouteGroupBuilder builder)
    {
        public RouteGroupBuilder ConfirmDisable2FAEndpoint()
        {
            builder.MapPost("2fa/disable/confirm", async ([FromBody] ConfirmDisable2FARequest request,
                    CancellationToken cancellationToken, [FromServices] IMediator mediator) =>
                {
                    var command = new ConfirmDisable2FACommand(request.Code);
                    var result = await mediator.Send<ConfirmDisable2FACommand, Result>(command, cancellationToken);

                    return result.ToResponse();
                })
                .RequireAuthorization()
                .Produces<Result>()
                .WithName("ConfirmDisable2FA");

            return builder;
        }
    }
}