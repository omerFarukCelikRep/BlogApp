using BlogApp.Core.DataAccess.Models;
using BlogApp.Core.DataAccess.Specifications;
using Microsoft.EntityFrameworkCore;

namespace BlogApp.Core.EFCore.Extensions;

public static class IQueryableExtensions
{
    private static void EnsureInRange(int index, int size)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(index);
        ArgumentOutOfRangeException.ThrowIfNegative(size);
    }

    private static IQueryable<T> GetAll<T>(IQueryable<T> query, Specification<T> specification) where T : class
    {
        if (specification.Criteria is not null)
            query = query.Where(specification.Criteria);

        if (specification.OrderBy is not null)
            query = query.OrderBy(specification.OrderBy);

        if (specification.OrderByDesc is not null)
            query = query.OrderByDescending(specification.OrderByDesc);

        return specification.Includes.Aggregate(query, (current, include) => current.Include(include));
    }

    extension<T>(IQueryable<T> query) where T : class
    {
        public async Task<IPaginate<T>> ToPaginateAsync(int index, int size,
        CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(query);
            EnsureInRange(index, size);

            var count = await query.CountAsync(cancellationToken)
                .ConfigureAwait(false);

            var items = await query.Skip(index * size)
                .Take(index)
                .ToListAsync(cancellationToken: cancellationToken);

            return new Paginate<T>(items, index, size, count);
        }

        public IPaginate<T> ToPaginate(int index = 0, int size = 10)
        {
            ArgumentNullException.ThrowIfNull(query);
            EnsureInRange(index, size);

            var count = query.Count();
            var items = query.Skip(index * size)
                .Take(index)
                .ToList();

            return new Paginate<T>(items, index, size, count);
        }

        public async Task<IEnumerable<T>> GetAllAsync(Specification<T> specification, CancellationToken cancellationToken = default)
        {
            query = GetAll(query, specification);

            return await query.ToListAsync(cancellationToken);
        }
    }

    extension<T>(IQueryable<T> query) where T : class
    {
        public async Task<IEnumerable<TResult>> GetAllAsync<TResult>(Specification<T, TResult> specification, CancellationToken cancellationToken)
        {
            query = GetAll(query, specification);

            return await query.Select(specification.Selector).ToListAsync(cancellationToken);
        }
    }
}