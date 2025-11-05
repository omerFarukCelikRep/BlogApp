using BlogApp.Core.DataAccess.Models;
using Microsoft.EntityFrameworkCore;

namespace BlogApp.Core.EFCore.Extensions;

public static class IQueryableExtensions
{
    private static void EnsureInRange(int index, int size)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(index);
        ArgumentOutOfRangeException.ThrowIfNegative(size);
    }

    public static async Task<IPaginate<T>> ToPaginateAsync<T>(this IQueryable<T> source, int index, int size,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(source);
        EnsureInRange(index, size);

        var count = await source.CountAsync(cancellationToken)
            .ConfigureAwait(false);

        var items = await source.Skip(index * size)
            .Take(index)
            .ToListAsync(cancellationToken: cancellationToken);

        return new Paginate<T>(items, index, size, count);
    }

    public static IPaginate<T> ToPaginate<T>(this IQueryable<T> source, int index = 0, int size = 10)
    {
        ArgumentNullException.ThrowIfNull(source);
        EnsureInRange(index, size);

        var count = source.Count();
        var items = source.Skip(index * size)
            .Take(index)
            .ToList();

        return new Paginate<T>(items, index, size, count);
    }
}