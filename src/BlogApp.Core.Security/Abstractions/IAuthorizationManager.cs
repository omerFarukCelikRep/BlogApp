using BlogApp.Core.Security.Enums;

namespace BlogApp.Core.Security.Abstractions;

public interface IAuthorizationManager
{
    Task<bool> AuthorizeByRoleAsync(Role requiredRole, CancellationToken cancellationToken = default);

    Task<bool> AuthorizeByPermissionAsync(IReadOnlyList<string> requiredPermissions, bool isRequireAll = false,
        CancellationToken cancellationToken = default);
}