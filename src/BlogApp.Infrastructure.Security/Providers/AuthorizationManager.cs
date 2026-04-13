using BlogApp.Core.Security.Abstractions;
using Role = BlogApp.Core.Security.Enums.Role;

namespace BlogApp.Infrastructure.Security.Providers;

public sealed class AuthorizationManager(IDomainPrincipal domainPrincipal)
    : IAuthorizationManager
{
    public Task<bool> AuthorizeByRoleAsync(Role requiredRole,
        CancellationToken cancellationToken = default)
    {
        if (cancellationToken.IsCancellationRequested)
            return Task.FromCanceled<bool>(cancellationToken);

        return Task.FromResult(domainPrincipal.IsInRole(requiredRole));
    }

    public Task<bool> AuthorizeByPermissionAsync(IReadOnlyList<string> requiredPermission, bool isRequireAll = false,
        CancellationToken cancellationToken = default)
    {
        if (cancellationToken.IsCancellationRequested)
            return Task.FromCanceled<bool>(cancellationToken);

        return Task.FromResult(isRequireAll
            ? domainPrincipal.HasAllPermissions([..requiredPermission])
            : domainPrincipal.HasAnyPermission([..requiredPermission]));
    }
}