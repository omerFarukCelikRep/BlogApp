using BlogApp.Api.Endpoints.Auth.Requests;
using BlogApp.Api.Endpoints.Shared.Responses;
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
        public RouteHandlerBuilder RegisterEndpoints()
        {
            return builder.MapPost("/register",
                    async ([FromBody] RegisterRequest request, CancellationToken cancellationToken,
                        [FromServices] IMediator mediator) =>
                    {
                        var command = (RegisterCommand)request;
                        var result = await mediator.Send<RegisterCommand, Result>(command, cancellationToken);

                        return result.ToResponse();
                    })
                .AllowAnonymous()
                .WithName("Register")
                .WithTags("Auth");
        }
    }
}