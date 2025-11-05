using BlogApp.Core.DataAccess.Entities;

namespace BlogApp.Core.DataAccess.Repositories;

public interface IAsyncDeletableRepository<TEntity, TId> : IAsyncRepository
    where TEntity : BaseEntity<TId> 
    where TId : struct
{
    Task DeleteAsync(TEntity entity, CancellationToken cancellationToken = default);
}