using BlogApp.Api.Extensions;
using BlogApp.Application.EmailConfirmations.Commands;
using BlogApp.Core.Mediator.Abstractions;
using BlogApp.Core.Results;
using Microsoft.AspNetCore.Mvc;

namespace BlogApp.Api.Endpoints.Auth;

public static class ConfirmEmailEndpoint
{
    extension(RouteGroupBuilder builder)
    {
        public RouteGroupBuilder ConfirmEmailEndpoint()
        {
            builder.MapPost("confirm-email",
                    async ([FromQuery] string token, CancellationToken cancellationToken,
                        [FromServices] IMediator mediator) =>
                    {
                        var command = new ConfirmEmailConfirmationCommand(token);
                        var result =
                            await mediator.Send<ConfirmEmailConfirmationCommand, Result>(command, cancellationToken);

                        return result.ToResponse();
                    })
                .AllowAnonymous()
                .Produces<Result>()
                .Produces<Result>(StatusCodes.Status400BadRequest)
                .WithName("ConfirmEmail");

            return builder;
        }
    }
}