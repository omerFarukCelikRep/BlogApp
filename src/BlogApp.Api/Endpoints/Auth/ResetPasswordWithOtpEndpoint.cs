using BlogApp.Api.Endpoints.Auth.Requests;
using BlogApp.Api.Extensions;
using BlogApp.Api.Filters;
using BlogApp.Application.Auth.Commands;
using BlogApp.Core.Mediator.Abstractions;
using BlogApp.Core.Results;
using Microsoft.AspNetCore.Mvc;

namespace BlogApp.Api.Endpoints.Auth;

public static class ResetPasswordWithOtpEndpoint
{
    extension(RouteGroupBuilder builder)
    {
        public RouteGroupBuilder ResetPasswordWithOtpEndpoint()
        {
            builder.MapPost("reset-password-otp",
                    async (ResetPasswordWithOtpRequest request, CancellationToken cancellationToken,
                        [FromServices] IMediator mediator) =>
                    {
                        var command = (ResetPasswordWithOtpCommand)request;
                        var result =
                            await mediator.Send<ResetPasswordWithOtpCommand, Result>(command, cancellationToken);

                        return result.ToResponse();
                    })
                .AllowAnonymous()
                .Produces<Result>()
                .WithName("ResetPasswordWithOtp")
                .AddEndpointFilter<XssProtectionEndpointFilter>();

            return builder;
        }
    }
}