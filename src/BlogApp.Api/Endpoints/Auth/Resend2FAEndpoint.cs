using BlogApp.Api.Extensions;
using BlogApp.Application.Auth.Commands;
using BlogApp.Core.Mediator.Abstractions;
using BlogApp.Core.Results;
using BlogApp.Domain.Models.Auth;
using Microsoft.AspNetCore.Mvc;

namespace BlogApp.Api.Endpoints.Auth;

public static class Resend2FAEndpoint
{
    extension(RouteGroupBuilder builder)
    {
        public RouteGroupBuilder Resend2FAEndpoint()
        {
            builder.MapPost("2fa/resend",
                    async (CancellationToken cancellationToken, [FromServices] IMediator mediator) =>
                    {
                        var command = new Send2FAOtpCommand();
                        var result = await mediator.Send<Send2FAOtpCommand, Result>(command, cancellationToken);

                        return result.ToResponse();
                    })
                .RequireAuthorization()
                .Produces<Result<LoginResult>>()
                .WithName("Resend2FAOtp");

            return builder;
        }
    }
}