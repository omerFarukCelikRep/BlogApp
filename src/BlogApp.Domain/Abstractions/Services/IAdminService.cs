using BlogApp.Core.Results;
using BlogApp.Domain.Models.Admins;

namespace BlogApp.Domain.Abstractions.Services;

public interface IAdminService
{
    Task<Result<DashboardStatsResult>> GetDashboardStatsAsync(CancellationToken cancellationToken = default);

    Task<PaginatedResult<AdminUserResult>> GetAllAsync(GetAllAdminsArgs args,
        CancellationToken cancellationToken = default);

    Task<Result> BanUserAsync(BanUserArgs args, CancellationToken cancellationToken = default);
    Task<Result> AssignRoleAsync(AssignRoleArgs args, CancellationToken cancellationToken = default);
}