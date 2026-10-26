using BlogApp.Core.Caching.Markers;
using BlogApp.Core.Security.Attributes;
using BlogApp.Core.Security.Enums;
using BlogApp.Domain.Models.Admins;

namespace BlogApp.Application.Admins.Queries;

[Authorize(Role.Admin)]
public record GetDashboardStatsQuery : IRequest<Result<DashboardStatsResult>>, ICacheable
{
    public string Key => "admin:dashboard:stats";
    public TimeSpan? Expiry => TimeSpan.FromMinutes(5);
}