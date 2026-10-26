using BlogApp.Core.Caching.Markers;
using BlogApp.Domain.Models.Blogs;

namespace BlogApp.Application.Blogs.Queries;

public record GetAllByAuthorQuery(string Username, int? Page, int? PageSize, string? SortBy)
    : GetBlogByAuthorArgs(Username: Username, Page, PageSize, SortBy),
        IRequest<PaginatedResult<BlogSummaryResult>>, ICacheable
{
    public string Key =>
        $"authors:blogs:{Username}:{Page}:{PageSize}:{SortBy}";

    public TimeSpan? Expiry => TimeSpan.FromMinutes(5);
}