using BlogApp.Core.Results;
using BlogApp.Core.Security.Abstractions;
using BlogApp.Domain.Abstractions.Repositories;
using BlogApp.Domain.Constants;
using Role = BlogApp.Core.Security.Enums.Role;

namespace BlogApp.Infrastructure.Security.Providers;

public sealed class RoleManager(IRoleRepository roleRepository, IUserRepository userRepository) : IRoleManager
{
    public async Task<IReadOnlyList<string>> GetUserRolesAsync(Guid userId,
        CancellationToken cancellationToken = default)
    {
        var roles = await roleRepository.GetUserRolesAsync(userId, cancellationToken);

        return [..roles.Select(x => x.Name)];
    }

    public async Task<IReadOnlyList<string>> GetUserPermissionsAsync(Guid userId,
        CancellationToken cancellationToken = default)
    {
        var permissions = await roleRepository.GetUserPermissionsAsync(userId, cancellationToken);

        return [..permissions.Select(x => x.ToString())];
    }

    public async Task<Result> AssignRoleAsync(Guid userId, Role role, CancellationToken cancellationToken = default)
    {
        var user = await userRepository.GetByIdAsync(userId, true, cancellationToken);
        if (user is null)
            return Result.Failed(Errors.User.NotFound, 404);

        var roleFromDb = await roleRepository.GetAsync(x => x.Name == role.ToString(), true, cancellationToken);
        if (roleFromDb is null)
            return Result.Failed(Errors.Role.NotFound, 404);

        var alreadyAssigned = user.UserRoles.Any(x => x.RoleId == roleFromDb.Id);
        if (alreadyAssigned)
            return Result.Failed(Errors.Role.AlreadyAssigned, 400);

        user.UserRoles.Add(new()
        {
            UserId = user.Id,
            RoleId = roleFromDb.Id
        });

        await userRepository.UpdateAsync(user, cancellationToken);
        await userRepository.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }

    public async Task<Result> RevokeRoleAsync(Guid userId, string role, CancellationToken cancellationToken = default)
    {
        var user = await userRepository.GetByIdAsync(userId, true, cancellationToken);
        if (user is null)
            return Result.Failed(Errors.User.NotFound, 404);

        var roleFromDb = await roleRepository.GetAsync(x => string.Equals(x.Name, role), true, cancellationToken);
        if (roleFromDb is null)
            return Result.Failed(Errors.Role.NotFound, 404);

        var userRole = user.UserRoles.FirstOrDefault(x => x.RoleId == roleFromDb.Id);
        if (userRole is null)
            return Result.Failed(Errors.Role.NotAssigned, 400);

        user.UserRoles.Remove(userRole);
        await userRepository.UpdateAsync(user, cancellationToken);
        await userRepository.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    public async Task<bool> HasRoleAsync(Guid userId, string role, CancellationToken cancellationToken = default)
    {
        return await roleRepository.AnyAsync(
            x => string.Equals(x.Name, role) && x.UserRoles.Any(r => r.UserId == userId), cancellationToken);
    }

    public async Task<IReadOnlyList<Role>> GetAllRolesAsync(CancellationToken cancellationToken = default)
    {
        var roles = await roleRepository.GetAllAsync(false, cancellationToken);

        return [..roles.Select(x => Enum.Parse<Role>(x.Name))];
    }

    public async Task<IReadOnlyList<string>> GetPermissionsAsync(Role role,
        CancellationToken cancellationToken = default)
    {
        var roleFromDb = await roleRepository.GetByNameAsync(role, false, cancellationToken);
        if (roleFromDb is null)
            return [];

        return [..roleFromDb.RolePermissions.Select(x => x.Permission!.ToString())];
    }

    public async Task<bool> HasPermissionAsync(Guid userId, string permission,
        CancellationToken cancellationToken = default)
    {
        return await roleRepository.AnyAsync(x => x.UserRoles.Any(r => r.UserId == userId
                                                                       && x.RolePermissions.Any(p =>
                                                                           p.Permission != null
                                                                           && p.Permission.ToString() == permission)),
            cancellationToken);
    }
}