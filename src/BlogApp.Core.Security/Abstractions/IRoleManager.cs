using BlogApp.Core.Security.Enums;

namespace BlogApp.Core.Security.Abstractions;

public interface IRoleManager
{
    Task<IReadOnlyList<Role>> GetAllRolesAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Permission>> GetPermissionsAsync(Role role, CancellationToken cancellationToken = default);
    Task<bool> RoleHasPermissionAsync(Role role, Permission permission, CancellationToken cancellationToken = default);
}