using BlogApp.Api.Endpoints.Auth.Requests;
using BlogApp.Api.Extensions;
using BlogApp.Application.Auth.Commands;
using BlogApp.Core.Mediator.Abstractions;
using BlogApp.Core.Results;
using Microsoft.AspNetCore.Mvc;

namespace BlogApp.Api.Endpoints.Auth;

public static class RegisterEndpoint
{
    extension(RouteGroupBuilder builder)
    {
        public RouteGroupBuilder RegisterEndpoint()
        {
            builder.MapPost("/register",
                    async ([FromBody] RegisterRequest request, CancellationToken cancellationToken,
                        [FromServices] IMediator mediator) =>
                    {
                        var command = (RegisterCommand)request;
                        var result = await mediator.Send<RegisterCommand, Result>(command, cancellationToken);

                        return result.ToResponse();
                    })
                .AllowAnonymous()
                .Produces<Result>()
                .Produces<Result>(StatusCodes.Status400BadRequest)
                .WithName("Register")
                .WithTags("Auth");

            return builder;
        }
    }
}