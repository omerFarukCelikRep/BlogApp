using System.Linq.Expressions;
using BlogApp.Core.DataAccess.Entities;

namespace BlogApp.Core.DataAccess.Repositories;

public interface IAsyncCountableRepository<TEntity, TId> : IAsyncRepository
    where TEntity : BaseEntity<TId>
    where TId : struct
{
    Task<int> CountAsync(CancellationToken cancellationToken = default);
    Task<int> CountAsync(Expression<Func<TEntity, bool>> expression, CancellationToken cancellationToken = default);
    Task<decimal> SumAsync(Expression<Func<TEntity, decimal>> selector, CancellationToken cancellationToken = default);
}