using BlogApp.Core.DataAccess.Entities;

namespace BlogApp.Core.DataAccess.Repositories;

public interface IAsyncInsertableRepository<TEntity, TId> : IAsyncRepository
    where TEntity : BaseEntity<TId>
    where TId : struct
{
    Task<TEntity> AddAsync(TEntity entity, CancellationToken cancellationToken = default);
    Task AddAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default);
}