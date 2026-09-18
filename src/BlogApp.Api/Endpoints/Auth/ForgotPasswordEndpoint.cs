using BlogApp.Api.Endpoints.Auth.Requests;
using BlogApp.Api.Extensions;
using BlogApp.Api.Filters;
using BlogApp.Application.Auth.Commands;
using BlogApp.Core.Mediator.Abstractions;
using BlogApp.Core.Results;
using Microsoft.AspNetCore.Mvc;

namespace BlogApp.Api.Endpoints.Auth;

public static class ForgotPasswordEndpoint
{
    extension(RouteGroupBuilder builder)
    {
        public RouteGroupBuilder ForgotPasswordEndpoint()
        {
            builder.MapPost("forgot-password",
                    async (ForgotPasswordRequest request, CancellationToken cancellationToken,
                        [FromServices] IMediator mediator) =>
                    {
                        var command = (ForgotPasswordCommand)request;
                        var result = await mediator.Send<ForgotPasswordCommand, Result>(command, cancellationToken);

                        return result.ToResponse();
                    })
                .AllowAnonymous()
                .Produces<Result>()
                .WithName("ForgotPassword")
                .AddEndpointFilter<XssProtectionEndpointFilter>();

            return builder;
        }
    }
}