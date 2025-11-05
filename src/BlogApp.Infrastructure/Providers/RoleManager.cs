using BlogApp.Core.Security.Abstractions;
using BlogApp.Core.Security.Enums;
using BlogApp.Domain.Abstractions.Repositories;
using Permission = BlogApp.Core.Security.Enums.Permission;
using Role = BlogApp.Core.Security.Enums.Role;

namespace BlogApp.Infrastructure.Providers;

public sealed class RoleManager(IRoleRepository roleRepository) : IRoleManager
{
    public async Task<IReadOnlyList<Role>> GetAllRolesAsync(CancellationToken cancellationToken = default)
    {
        var roles = await roleRepository.GetAllAsync(false, cancellationToken);

        return roles.Select(x => Enum.Parse<Role>(x.ToString()))
            .ToList()
            .AsReadOnly();
    }

    public async Task<IReadOnlyList<Permission>> GetPermissionsAsync(Role role,
        CancellationToken cancellationToken = default)
    {
        var existRole =
            await roleRepository.GetAsync(x => x.Name == nameof(role), cancellationToken: cancellationToken);

        return existRole!.RolePermissions.Select(x => Enum.Parse<Permission>(x.Permission!.ToString()))
            .ToList()
            .AsReadOnly();
    }

    public async Task<bool> RoleHasPermissionAsync(Role role, Permission permission,
        CancellationToken cancellationToken = default)
    {
        var existRole =
            await roleRepository.GetAsync(x => x.Name == nameof(role), cancellationToken: cancellationToken);

        return existRole?.RolePermissions.Any(x => x.Permission?.ToString() == nameof(permission)) ?? false;
    }
}