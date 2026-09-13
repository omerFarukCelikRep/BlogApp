using BlogApp.Api.Endpoints.Auth.Requests;
using BlogApp.Api.Extensions;
using BlogApp.Application.EmailConfirmations.Commands;
using BlogApp.Core.Mediator.Abstractions;
using BlogApp.Core.Results;
using Microsoft.AspNetCore.Mvc;

namespace BlogApp.Api.Endpoints.Auth;

public static class ConfirmEmailWithOtpEndpoint
{
    extension(RouteGroupBuilder builder)
    {
        public RouteGroupBuilder ConfirmEmailWithOtpEndpoint()
        {
            builder.MapPost("confirm-email-otp",
                    async ([FromBody] ConfirmOtpRequest request, CancellationToken cancellationToken,
                        [FromServices] IMediator mediator) =>
                    {
                        var command = (ConfirmEmailConfirmationWithOtpCommand)request;
                        var result =
                            await mediator.Send<ConfirmEmailConfirmationWithOtpCommand, Result>(command,
                                cancellationToken);

                        return result.ToResponse();
                    })
                .RequireAuthorization()
                .Produces<Result>()
                .Produces<Result>(StatusCodes.Status400BadRequest)
                .WithName("ConfirmEmailWithOtp");

            return builder;
        }
    }
}