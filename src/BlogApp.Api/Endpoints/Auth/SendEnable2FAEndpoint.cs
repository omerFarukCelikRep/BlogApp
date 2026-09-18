using BlogApp.Api.Endpoints.Auth.Requests;
using BlogApp.Api.Extensions;
using BlogApp.Application.Auth.Commands;
using BlogApp.Core.Mediator.Abstractions;
using BlogApp.Core.Results;
using Microsoft.AspNetCore.Mvc;

namespace BlogApp.Api.Endpoints.Auth;

public static class SendEnable2FAEndpoint
{
    extension(RouteGroupBuilder builder)
    {
        public RouteGroupBuilder SendEnable2FAEndpoint()
        {
            builder.MapPost("2fa/enable/send",
                    async ([FromBody] SendEnable2FARequest request, CancellationToken cancellationToken,
                        [FromServices] IMediator mediator) =>
                    {
                        var command = new SendEnable2FACommand(request.PhoneNumber);
                        var result = await mediator.Send<SendEnable2FACommand, Result>(command, cancellationToken);

                        return result.ToResponse();
                    })
                .RequireAuthorization()
                .Produces<Result>()
                .WithName("SendEnable2FA");

            return builder;
        }
    }
}