using BlogApp.Core.Caching.Markers;
using BlogApp.Core.Search.Models;

namespace BlogApp.Application.Search;

public record SearchQuery(
    string Q,
    string Type,
    string? Tag,
    string? Author,
    string? DateFrom,
    string? DateTo,
    string SortBy,
    int Page,
    int PageSize) : SearchFilter(Q.Trim(),
        Type,
        Tag,
        Author,
        DateFrom,
        DateTo,
        SortBy,
        Page,
        PageSize),
    IRequest<Result<SearchResult>>,
    ICacheable
{
    public string Key => $"search:{Q}:{Type}:{Tag}:{Author}:{DateFrom}:{DateTo}:{SortBy}:{Page}:{PageSize}";

    public TimeSpan? Expiry => TimeSpan.FromMinutes(2);
}