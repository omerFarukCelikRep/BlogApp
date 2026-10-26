using BlogApp.Api.Extensions;
using BlogApp.Application.Admins.Queries;
using BlogApp.Core.Mediator.Abstractions;
using BlogApp.Core.Results;
using BlogApp.Domain.Models.Admins;
using Microsoft.AspNetCore.Mvc;

namespace BlogApp.Api.Endpoints.Admins;

public static class DashboardEndpoint
{
    extension(RouteGroupBuilder builder)
    {
        public RouteGroupBuilder DashboardEndpoints()
        {
            builder.MapGet("dashboard",
                    async (CancellationToken cancellationToken, [FromServices] IMediator mediator) =>
                    {
                        var result =
                            await mediator.Send<GetDashboardStatsQuery, Result<DashboardStatsResult>>(
                                new GetDashboardStatsQuery(), cancellationToken);

                        return result.ToResponse();
                    })
                .RequireAuthorization()
                .Produces<Result<DashboardStatsResult>>()
                .WithName("GetDashboardStats");

            return builder;
        }
    }
}