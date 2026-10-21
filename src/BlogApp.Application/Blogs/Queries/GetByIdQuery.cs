using BlogApp.Core.Caching.Markers;
using BlogApp.Domain.Models.Blogs;

namespace BlogApp.Application.Blogs.Queries;

public record GetByIdQuery(int Id) : GetBlogByIdArgs(Id), IRequest<Result<BlogDetailsResult>>, ICacheable
{
    public string Key => $"blogs:details:{Id}";
    public TimeSpan? Expiry => TimeSpan.FromMinutes(5);
}