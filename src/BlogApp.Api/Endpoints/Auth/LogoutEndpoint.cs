using BlogApp.Api.Extensions;
using BlogApp.Application.Auth.Commands;
using BlogApp.Core.Mediator.Abstractions;
using BlogApp.Core.Results;
using Microsoft.AspNetCore.Mvc;

namespace BlogApp.Api.Endpoints.Auth;

public static class LogoutEndpoint
{
    extension(RouteGroupBuilder builder)
    {
        public RouteGroupBuilder LogoutEndpoint()
        {
            builder.MapPost("/logout",
                    async (CancellationToken cancellationToken,
                        [FromServices] IMediator mediator) =>
                    {
                        var result = await mediator.Send<LogoutCommand, Result>(new LogoutCommand(), cancellationToken);

                        return result.ToResponse();
                    })
                .RequireAuthorization()
                .Produces<Result>()
                .Produces<Result>(StatusCodes.Status401Unauthorized)
                .WithName("Logout")
                .WithTags("Auth");

            return builder;
        }
    }
}