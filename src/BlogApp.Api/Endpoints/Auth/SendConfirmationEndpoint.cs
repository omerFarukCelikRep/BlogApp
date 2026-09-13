using BlogApp.Api.Extensions;
using BlogApp.Application.EmailConfirmations.Commands;
using BlogApp.Core.Mediator.Abstractions;
using BlogApp.Core.Results;
using Microsoft.AspNetCore.Mvc;

namespace BlogApp.Api.Endpoints.Auth;

public static class SendConfirmationEndpoint
{
    extension(RouteGroupBuilder builder)
    {
        public RouteGroupBuilder SendConfirmationEndpoint()
        {
            builder.MapPost("send-confirmation",
                    async (CancellationToken cancellationToken, [FromServices] IMediator mediator) =>
                    {
                        var command = new SendEmailConfirmationCommand();
                        var result =
                            await mediator.Send<SendEmailConfirmationCommand, Result>(command, cancellationToken);

                        return result.ToResponse();
                    })
                .RequireAuthorization()
                .Produces<Result>()
                .WithName("SendConfirmation");

            return builder;
        }
    }
}