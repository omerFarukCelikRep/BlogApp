using BlogApp.Api.Endpoints.Auth.Requests;
using BlogApp.Api.Extensions;
using BlogApp.Application.Auth.Commands;
using BlogApp.Core.Mediator.Abstractions;
using BlogApp.Core.Results;
using Microsoft.AspNetCore.Mvc;

namespace BlogApp.Api.Endpoints.Auth;

public static class ConfirmEnable2FAEndpoint
{
    extension(RouteGroupBuilder builder)
    {
        public RouteGroupBuilder ConfirmEnable2FAEndpoint()
        {
            builder.MapPost("2fa/enable/confirm",
                    async ([FromBody] ConfirmEnable2FARequest request, CancellationToken cancellationToken,
                        [FromServices] IMediator mediator) =>
                    {
                        var command = new ConfirmEnable2FACommand(request.Code, request.PhoneNumber);
                        var result = await mediator.Send<ConfirmEnable2FACommand, Result>(command, cancellationToken);

                        return result.ToResponse();
                    })
                .RequireAuthorization()
                .Produces<Result>()
                .WithName("ConfirmEnable2FA");

            return builder;
        }
    }
}