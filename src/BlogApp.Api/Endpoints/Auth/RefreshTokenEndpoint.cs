using BlogApp.Api.Endpoints.Auth.Requests;
using BlogApp.Api.Endpoints.Shared.Responses;
using BlogApp.Api.Extensions;
using BlogApp.Application.Auth.Commands;
using BlogApp.Core.Mediator.Abstractions;
using BlogApp.Core.Results;
using BlogApp.Domain.Models.RefreshTokens;
using Microsoft.AspNetCore.Mvc;

namespace BlogApp.Api.Endpoints.Auth;

public static class RefreshTokenEndpoint
{
    extension(RouteGroupBuilder builder)
    {
        public RouteGroupBuilder RefreshTokenEndpoints()
        {
            builder.MapPost("/refresh-token",
                async (RefreshTokenRequest request, CancellationToken cancellationToken,
                    [FromServices] IMediator mediator) =>
                {
                    var command = (RefreshTokenCommand)request;
                    var result =
                        await mediator.Send<RefreshTokenCommand, Result<RefreshTokenResult>>(command,
                            cancellationToken);

                    return result.ToResponse();
                })
                .RequireAuthorization()
                .WithName("RefreshToken")
                .WithTags("Auth");

            return builder;
        }
    }
}