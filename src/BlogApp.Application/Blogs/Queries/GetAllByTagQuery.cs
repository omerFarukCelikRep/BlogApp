using BlogApp.Core.Caching.Markers;
using BlogApp.Core.DataAccess.Models;
using BlogApp.Domain.Models.Blogs;

namespace BlogApp.Application.Blogs.Queries;

public record GetAllByTagQuery(
    string Slug,
    int? Page,
    int? PageSize,
    string? SortBy) : GetBlogByTagArgs(Slug, Page, PageSize, SortBy), IRequest<PaginatedResult<BlogSummaryResult>>,
    ICacheable
{
    public string Key => $"tags:blogs:{Slug}:{Page}:{PageSize}:{SortBy}";
    public TimeSpan? Expiry => TimeSpan.FromMinutes(5);
}