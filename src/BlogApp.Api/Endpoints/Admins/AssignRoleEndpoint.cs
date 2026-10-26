using BlogApp.Api.Endpoints.Admins.Requests;
using BlogApp.Api.Extensions;
using BlogApp.Application.Admins.Commands;
using BlogApp.Core.Mediator.Abstractions;
using BlogApp.Core.Results;
using Microsoft.AspNetCore.Mvc;

namespace BlogApp.Api.Endpoints.Admins;

public static class AssignRoleEndpoint
{
    extension(RouteGroupBuilder builder)
    {
        public RouteGroupBuilder AssignRoleEndpoint()
        {
            builder.MapPut("{id:guid}/role",
                    async ([FromBody] AssignRoleRequest request, CancellationToken cancellationToken,
                        [FromServices] IMediator mediator) =>
                    {
                        var command = (AssignRoleCommand)request;
                        var result = await mediator.Send<AssignRoleCommand, Result>(command, cancellationToken);

                        return result.ToResponse();
                    })
                .RequireAuthorization()
                .Produces<Result>()
                .WithName("AssignRole");

            return builder;
        }
    }
}