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

        return await GetAsync(x => x.Name.Equals(role), tracking, cancellationToken);
    }
}