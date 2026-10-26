using BlogApp.Core.Caching.Markers;
using BlogApp.Domain.Models.Blogs;

namespace BlogApp.Application.Blogs.Queries;

public record GetAllByCategoryQuery(
    string Category,
    int? Page = null,
    int? PageSize = null,
    string? SortBy = null,
    bool IncludeChildren = false)
    : GetBlogByCategoryArgs(Category, Page, PageSize, SortBy, IncludeChildren),
        IRequest<PaginatedResult<BlogSummaryResult>>, ICacheable
{
    public string Key =>
        $"categories:blogs:{Category}:{Page}:{PageSize}:{SortBy}:{IncludeChildren}";

    public TimeSpan? Expiry => TimeSpan.FromMinutes(5);
}