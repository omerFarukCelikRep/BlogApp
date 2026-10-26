using BlogApp.Api.Endpoints.Admins.Requests;
using BlogApp.Api.Extensions;
using BlogApp.Application.Admins.Queries;
using BlogApp.Core.Mediator.Abstractions;
using BlogApp.Core.Results;
using BlogApp.Domain.Models.Admins;
using Microsoft.AspNetCore.Mvc;

namespace BlogApp.Api.Endpoints.Admins;

public static class UserAdminEndpoint
{
    extension(RouteGroupBuilder builder)
    {
        public RouteGroupBuilder UserAdminEndpoint()
        {
            builder.MapGet(string.Empty,
                    async ([AsParameters] GetAllRequest request, CancellationToken cancellationToken,
                        [FromServices] IMediator mediator) =>
                    {
                        var query = (GetAllQuery)request;
                        var result =
                            await mediator.Send<GetAllQuery, PaginatedResult<AdminUserResult>>(query,
                                cancellationToken);

                        return result.ToResponse();
                    })
                .RequireAuthorization()
                .Produces<PaginatedResult<AdminUserResult>>()
                .WithName("GetAllAdmin");

            return builder;
        }
    }
}