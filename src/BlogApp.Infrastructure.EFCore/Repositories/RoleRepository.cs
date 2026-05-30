using BlogApp.Domain.Abstractions.Repositories;
using BlogApp.Infrastructure.EFCore.Contexts;

namespace BlogApp.Infrastructure.EFCore.Repositories;

public class RoleRepository(BlogAppDbContext context) : EFBaseRepository<Role, int>(context), IRoleRepository
{
    public async Task<Role?> GetByNameAsync(Core.Security.Enums.Role role, bool tracking = true,
        CancellationToken cancellationToken = default)
    {
        if (cancellationToken.IsCancellationRequested)
            return await Task.FromCanceled<Role?>(cancellationToken);

        return await GetAsync(x => x.Name.Equals(role.ToString()), tracking, cancellationToken);
    }

    public async Task<IEnumerable<Role>> GetUserRolesAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        if (cancellationToken.IsCancellationRequested)
            return await Task.FromCanceled<IEnumerable<Role>>(cancellationToken);

        return await GetAllAsync(x => x.UserRoles.Any(ur => ur.UserId == userId), tracking: false,
            cancellationToken);
    }

    public async Task<IEnumerable<Permission>> GetUserPermissionsAsync(Guid userId,
        CancellationToken cancellationToken = default)
    {
        if (cancellationToken.IsCancellationRequested)
            return await Task.FromCanceled<IEnumerable<Permission>>(cancellationToken);

        return await GetAll(false)
            .Where(x => x.UserRoles.Any(ur => ur.UserId == userId))
            .Include(x => x.RolePermissions)
            .ThenInclude(x => x.Permission)
            .SelectMany(x => x.RolePermissions)
            .Select(x => x.Permission!)
            .Distinct()
            .ToListAsync(cancellationToken);
    }
}