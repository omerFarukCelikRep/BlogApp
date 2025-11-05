using BlogApp.Core.Security.Enums;

namespace BlogApp.Core.Security.Abstractions;

public interface IAuthorizationManager
{
    Task<bool> AuthorizeByRoleAsync(Guid userId, Role requiredRole, CancellationToken cancellationToken = default);

    Task<bool> AuthorizeByPermissionAsync(Guid userId, Permission requiredPermission,
        CancellationToken cancellationToken = default);
}