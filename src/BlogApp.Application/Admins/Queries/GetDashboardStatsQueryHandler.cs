using BlogApp.Domain.Abstractions.Services;
using BlogApp.Domain.Models.Admins;

namespace BlogApp.Application.Admins.Queries;

public class GetDashboardStatsQueryHandler(IAdminService adminService) : IRequestHandler<GetDashboardStatsQuery,Result<DashboardStatsResult>>
{
    public async Task<Result<DashboardStatsResult>> Handle(GetDashboardStatsQuery request, CancellationToken cancellationToken = default)
    {
        return await adminService.GetDashboardStatsAsync(cancellationToken);
    }
}