using BlogApp.Api.Endpoints.Auth.Requests;
using BlogApp.Api.Endpoints.Shared.Responses;
using BlogApp.Application.Auth.Commands;
using BlogApp.Core.Mediator.Abstractions;
using BlogApp.Core.Results;
using BlogApp.Domain.Models.Auth;
using Microsoft.AspNetCore.Mvc;

namespace BlogApp.Api.Endpoints.Auth;

public static class LoginEndpoint
{
    extension(RouteGroupBuilder builder)
    {
        public RouteHandlerBuilder LoginEndpoints()
        {
            return builder.Map("login",
                    async (LoginRequest request, CancellationToken cancellationToken, [FromServices] IMediator mediator) =>
                    {
                        var command = (LoginCommand)request;
                        var result = await mediator.Send(command, cancellationToken);

                        return new Response<LoginResult>(result);
                    })
                .HasApiVersion(1)
                .AllowAnonymous();
        }
    }
}