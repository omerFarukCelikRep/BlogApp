using BlogApp.Core.DataAccess.Entities;
using BlogApp.Core.DataAccess.Models;
using BlogApp.Core.DataAccess.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using BlogApp.Core.EFCore.Extensions;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;

namespace BlogApp.Core.EFCore.Repositories;

public class EFBaseRepository<TEntity, TId>(DbContext context)
    : IAsyncPaginateRepository<TEntity, TId>, IAsyncFindableRepository<TEntity, TId>,
        IAsyncOrderableRepository<TEntity, TId>, IAsyncQueryableRepository<TEntity, TId>,
        IAsyncInsertableRepository<TEntity, TId>, IAsyncUpdateableRepository<TEntity, TId>,
        IAsyncDeletableRepository<TEntity, TId>
    where TEntity : BaseEntity<TId>
    where TId : struct
{
    private readonly DbSet<TEntity> _table = context.Set<TEntity>();
    private IDbContextTransaction? _currentTransaction;

    private async Task DisposeTransactionAsync()
    {
        if (_currentTransaction != null)
        {
            await _currentTransaction.DisposeAsync();
            _currentTransaction = null;
        }
    }

    protected IQueryable<TEntity> GetAll(bool tracking = true)
    {
        var values = _table.AsQueryable<TEntity>();

        return tracking
            ? values
            : values.AsNoTracking();
    }

    public async Task<TEntity> AddAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        await _table.AddAsync(entity, cancellationToken);
        return entity;
    }

    public async Task AddAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
    {
        await _table.AddRangeAsync(entities, cancellationToken);
    }

    public Task<TEntity> UpdateAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        return cancellationToken.IsCancellationRequested
            ? Task.FromCanceled<TEntity>(cancellationToken)
            : Task.FromResult(_table.Update(entity).Entity);
    }

    public Task DeleteAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        return cancellationToken.IsCancellationRequested
            ? Task.FromCanceled(cancellationToken)
            : Task.FromResult(_table.Remove(entity));
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await context.SaveChangesAsync(cancellationToken);
    }

    public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        _currentTransaction ??= await context.Database.BeginTransactionAsync(cancellationToken);
    }

    public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(_currentTransaction);

        try
        {
            await SaveChangesAsync(cancellationToken);
            await _currentTransaction.CommitAsync(cancellationToken);
        }
        catch (Exception)
        {
            await RollbackTransactionAsync(cancellationToken);
            throw;
        }
        finally
        {
            await DisposeTransactionAsync();
        }
    }

    public async Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_currentTransaction is null)
            return;

        await _currentTransaction.RollbackAsync(cancellationToken);
        await DisposeTransactionAsync();
    }

    public async Task<IPaginate<TEntity>> GetAllAsPaginateAsync(int index = 0, int size = 10, bool tracking = true,
        CancellationToken cancellationToken = default)
    {
        return await GetAll(tracking).ToPaginateAsync(index, size, cancellationToken);
    }

    public async Task<IPaginate<TEntity>> GetAllAsPaginateAsync(Expression<Func<TEntity, bool>> expression,
        int index = 0, int size = 10, bool tracking = true, CancellationToken cancellationToken = default)
    {
        return await GetAll(tracking).Where(expression).ToPaginateAsync(index, size, cancellationToken);
    }

    public async Task<TEntity?> GetAsync(Expression<Func<TEntity, bool>> expression, bool tracking = true,
        CancellationToken cancellationToken = default)
    {
        return await GetAll(tracking).FirstOrDefaultAsync(expression, cancellationToken);
    }

    public async Task<TEntity?> GetByIdAsync(TId id, bool tracking = true,
        CancellationToken cancellationToken = default)
    {
        return await GetAll(tracking).FirstOrDefaultAsync(x => x.Id.Equals(id), cancellationToken);
    }

    public async Task<bool> AnyAsync(Expression<Func<TEntity, bool>>? expression = null,
        CancellationToken cancellationToken = default)
    {
        return expression is null
            ? await GetAll(false).AnyAsync(cancellationToken)
            : await GetAll(false).AnyAsync(expression, cancellationToken);
    }

    public async Task<IEnumerable<TEntity>> GetAllAsync<TKey>(Expression<Func<TEntity, TKey>> orderby,
        bool orderDesc = false, bool tracking = true, CancellationToken cancellationToken = default)
    {
        var query = GetAll(tracking);
        var orderedQuery = orderDesc
            ? query.OrderByDescending(orderby)
            : query.OrderBy(orderby);
        return await orderedQuery.ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<TEntity>> GetAllAsync<TKey>(Expression<Func<TEntity, TKey>> orderby,
        bool orderDesc = false, int takeCount = 0, bool tracking = true, CancellationToken cancellationToken = default)
    {
        var query = GetAll(tracking);
        var orderedQuery = orderDesc
            ? query.OrderByDescending(orderby)
            : query.OrderBy(orderby);
        return await orderedQuery.Take(takeCount).ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<TEntity>> GetAllAsync<TKey>(Expression<Func<TEntity, bool>> expression,
        Expression<Func<TEntity, TKey>> orderby, bool orderDesc = false, bool tracking = true,
        CancellationToken cancellationToken = default)
    {
        var query = GetAll(tracking).Where(expression);
        var orderedQuery = orderDesc
            ? query.OrderByDescending(orderby)
            : query.OrderBy(orderby);
        return await orderedQuery.ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<TEntity>> GetAllAsync<TKey>(Expression<Func<TEntity, bool>> expression,
        Expression<Func<TEntity, TKey>> orderby, bool orderDesc = false, int takeCount = 0, bool tracking = true,
        CancellationToken cancellationToken = default)
    {
        var query = GetAll(tracking).Where(expression);
        var orderedQuery = orderDesc
            ? query.OrderByDescending(orderby)
            : query.OrderBy(orderby);
        return await orderedQuery.Take(takeCount).ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<TEntity>> GetAllAsync(bool tracking = true,
        CancellationToken cancellationToken = default)
    {
        return await GetAll(tracking).ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<TEntity>> GetAllAsync(Expression<Func<TEntity, bool>> expression,
        bool tracking = true, CancellationToken cancellationToken = default)
    {
        return await GetAll().Where(expression).ToListAsync(cancellationToken);
    }
}