using BlogApp.Api.Endpoints.Auth.Requests;
using BlogApp.Api.Extensions;
using BlogApp.Application.Auth.Commands;
using BlogApp.Core.Mediator.Abstractions;
using BlogApp.Core.Results;
using Microsoft.AspNetCore.Mvc;

namespace BlogApp.Api.Endpoints.Auth;

public static class ResetPasswordEndpoint
{
    extension(RouteGroupBuilder builder)
    {
        public RouteGroupBuilder ResetPasswordEndpoint()
        {
            builder.MapPost("reset-password",
                    async (ResetPasswordRequest request, CancellationToken cancellationToken,
                        [FromServices] IMediator mediator) =>
                    {
                        var command = (ResetPasswordCommand)request;
                        var result = await mediator.Send<ResetPasswordCommand, Result>(command, cancellationToken);

                        return result.ToResponse();
                    })
                .AllowAnonymous()
                .Produces<Result>()
                .WithName("ResetPassword");

            return builder;
        }
    }
}