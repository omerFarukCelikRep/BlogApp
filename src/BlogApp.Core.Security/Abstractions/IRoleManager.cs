using BlogApp.Core.Results;
using BlogApp.Core.Security.Enums;

namespace BlogApp.Core.Security.Abstractions;

public interface IRoleManager
{
    /// <summary>
    /// Gets all roles assigned to a user.
    /// </summary>
    Task<IReadOnlyList<string>> GetUserRolesAsync(Guid userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all permissions for a user based on their assigned roles.
    /// </summary>
    Task<IReadOnlyList<string>> GetUserPermissionsAsync(Guid userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Assigns a role to a user.
    /// </summary>
    Task<Result> AssignRoleAsync(Guid userId, Role role, CancellationToken cancellationToken = default);

    /// <summary>
    /// Revokes a role from a user.
    /// </summary>
    Task<Result> RevokeRoleAsync(Guid userId, string role, CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if a user has a specific role.
    /// </summary>
    Task<bool> HasRoleAsync(Guid userId, string role, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Role>> GetAllRolesAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<string>> GetPermissionsAsync(Role role, CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if a user has a specific permission.
    /// </summary>
    Task<bool> HasPermissionAsync(Guid userId, string permission, CancellationToken cancellationToken = default);
}