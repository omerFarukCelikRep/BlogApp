using System.Linq.Expressions;
using BlogApp.Core.DataAccess.Entities;

namespace BlogApp.Core.DataAccess.Repositories;

public interface IAsyncFindableRepository<TEntity, TId> : IAsyncRepository
    where TEntity : BaseEntity<TId>
    where TId : struct
{
    Task<TEntity?> GetAsync(Expression<Func<TEntity, bool>> expression, bool tracking = true,
        CancellationToken cancellationToken = default);

    Task<TEntity?> GetByIdAsync(TId id, bool tracking = true, CancellationToken cancellationToken = default);

    Task<bool> AnyAsync(Expression<Func<TEntity, bool>>? expression = null,
        CancellationToken cancellationToken = default);
}