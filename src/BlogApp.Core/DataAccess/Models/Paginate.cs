using BlogApp.Core.Results;

namespace BlogApp.Core.DataAccess.Models;

public class Paginate<TModel> : IPaginate<TModel>
{
    public Paginate(IEnumerable<TModel> source, int index, int size)
    {
        ArgumentNullException.ThrowIfNull(source);

        if (source is IQueryable<TModel> queryable)
        {
            Index = index;
            Size = size;
            Count = queryable.Count();
            Pages = (int)Math.Ceiling(Count / (double)Size);

            Items = queryable.Take((Index * Size)..Size).ToList().AsReadOnly();
        }
        else
        {
            var enumerable = source as TModel[] ?? source.ToArray();
            Index = index;
            Size = size;
            Count = enumerable.Length;
            Pages = (int)Math.Ceiling(Count / (double)Size);
            Items = enumerable.Take((Index * Size)..Size).ToList();
        }
    }

    public Paginate(IEnumerable<TModel> source, int index, int size, int count) : this(source, index, size)
    {
        Count = count;
    }

    public int Index { get; }
    public int Size { get; }
    public int Count { get; }
    public int Pages { get; }
    public IReadOnlyCollection<TModel> Items { get; init; }
    public bool HasPrevious => Index * Size > 0;
    public bool HasNext => Index < Pages;

    public PaginatedResult<TModel> ToPagedResult() => new PaginatedResult<TModel>(Items, Index, Size, Count);
}