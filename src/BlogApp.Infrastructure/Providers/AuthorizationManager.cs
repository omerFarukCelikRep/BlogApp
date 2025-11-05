using BlogApp.Core.Security.Abstractions;
using BlogApp.Domain.Abstractions.Repositories;
using Permission = BlogApp.Core.Security.Enums.Permission;
using Role = BlogApp.Core.Security.Enums.Role;

namespace BlogApp.Infrastructure.Providers;

public sealed class AuthorizationManager(IRoleManager roleManager, IUserRepository userRepository)
    : IAuthorizationManager
{
    public async Task<bool> AuthorizeByRoleAsync(Guid userId, Role requiredRole,
        CancellationToken cancellationToken = default)
    {
        var user = await userRepository.GetByIdAsync(userId, false, cancellationToken);
        if (user is null)
            return false;

        var hasRole = user.Roles.Any(x => x.Role?.Name == requiredRole.ToString());
        return hasRole;
    }

    public async Task<bool> AuthorizeByPermissionAsync(Guid userId, Permission requiredPermission,
        CancellationToken cancellationToken = default)
    {
        var user = await userRepository.GetByIdAsync(userId, false, cancellationToken);
        if (user is null)
            return false;

        foreach (var userRole in user.Roles)
        {
            var parsed = Enum.TryParse<Role>(userRole.Role?.ToString(), out var role);
            if (!parsed)
                continue;

            var permittedRole = await roleManager.RoleHasPermissionAsync(role, requiredPermission, cancellationToken);
            if (!permittedRole)
                continue;

            return permittedRole;
        }

        return false;
    }
}