using Asp.Versioning;
using Asp.Versioning.Builder;
using BlogApp.Api.Endpoints.Auth.Requests;
using BlogApp.Api.Endpoints.Shared.Responses;
using BlogApp.Application.Auth.Commands;
using BlogApp.Core.Mediator.Abstractions;
using Microsoft.AspNetCore.Mvc;

namespace BlogApp.Api.Endpoints.Auth;

public static class RegisterEndpoint
{
    public static RouteHandlerBuilder RegisterEndpoints(this RouteGroupBuilder builder)
    {
        return builder.Map("/register",
                async (RegisterRequest request, CancellationToken cancellationToken,
                    [FromServices] IMediator mediator) =>
                {
                    var command = (RegisterCommand)request;
                    var result = await mediator.Send(command, cancellationToken);

                    return new Response(result);
                })
            .AllowAnonymous();
    }
}