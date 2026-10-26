using BlogApp.Domain.Abstractions.Services;
using BlogApp.Domain.Models.Admins;

namespace BlogApp.Application.Admins.Queries;

public class GetAllQueryHandler(IAdminService adminService) : IRequestHandler<GetAllQuery, PaginatedResult<AdminUserResult>>
{
    public async Task<PaginatedResult<AdminUserResult>> Handle(GetAllQuery request, CancellationToken cancellationToken = default)
    {
        return await adminService.GetAllAsync(request, cancellationToken);
    }
}