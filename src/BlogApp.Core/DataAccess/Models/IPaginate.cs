namespace BlogApp.Core.DataAccess.Models;

public interface IPaginate<TModel>
{
    int Index { get; }
    int Size { get; }
    int Count { get; }
    int Pages { get; }
    IReadOnlyCollection<TModel> Items { get; }
    bool HasPrevious { get; }
    bool HasNext { get; }
}