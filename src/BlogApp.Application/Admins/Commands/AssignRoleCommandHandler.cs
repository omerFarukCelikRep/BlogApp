using BlogApp.Domain.Abstractions.Services;

namespace BlogApp.Application.Admins.Commands;

public class AssignRoleCommandHandler(IAdminService adminService) : IRequestHandler<AssignRoleCommand, Result>
{
    public async Task<Result> Handle(AssignRoleCommand request, CancellationToken cancellationToken = default)
    {
        return await adminService.AssignRoleAsync(request, cancellationToken);
    }
}