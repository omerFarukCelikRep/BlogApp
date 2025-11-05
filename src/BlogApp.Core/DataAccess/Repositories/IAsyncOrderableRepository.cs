using System.Linq.Expressions;
using BlogApp.Core.DataAccess.Entities;

namespace BlogApp.Core.DataAccess.Repositories;

public interface IAsyncOrderableRepository<TEntity, TId> : IAsyncRepository
    where TEntity : BaseEntity<TId>
    where TId : struct
{
    Task<IEnumerable<TEntity>> GetAllAsync<TKey>(Expression<Func<TEntity, TKey>> orderby, bool orderDesc = false,
        bool tracking = true, CancellationToken cancellationToken = default);

    Task<IEnumerable<TEntity>> GetAllAsync<TKey>(Expression<Func<TEntity, TKey>> orderby, bool orderDesc = false,
        int takeCount = 0, bool tracking = true, CancellationToken cancellationToken = default);

    Task<IEnumerable<TEntity>> GetAllAsync<TKey>(Expression<Func<TEntity, bool>> expression,
        Expression<Func<TEntity, TKey>> orderby, bool orderDesc = false, bool tracking = true,
        CancellationToken cancellationToken = default);

    Task<IEnumerable<TEntity>> GetAllAsync<TKey>(Expression<Func<TEntity, bool>> expression,
        Expression<Func<TEntity, TKey>> orderby, bool orderDesc = false, int takeCount = 0, bool tracking = true,
        CancellationToken cancellationToken = default);
}