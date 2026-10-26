using BlogApp.Core.Caching.Markers;
using BlogApp.Domain.Models.Blogs;

namespace BlogApp.Application.Blogs.Commands;

public record IncrementReadCountCommand(int Id) : IncrementReadCountArgs(Id), IRequest<Result>, ICacheInvalidating
{
    public List<string> InvalidationKeys =>
    [
        $"blogs:detail:{Id}",
        "blogs:top-read",
    ];
}