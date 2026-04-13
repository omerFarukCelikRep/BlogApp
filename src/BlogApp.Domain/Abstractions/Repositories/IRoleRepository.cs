using BlogApp.Core.DataAccess.Repositories;

namespace BlogApp.Domain.Abstractions.Repositories;

public interface IRoleRepository : IAsyncQueryableRepository<Role, int>, IAsyncFindableRepository<Role, int>
{
    Task<Role?> GetByNameAsync(Core.Security.Enums.Role role, bool tracking = true,
        CancellationToken cancellationToken = default);
}