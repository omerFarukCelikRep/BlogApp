using BlogApp.Core.Caching.Markers;
using BlogApp.Core.Security.Attributes;
using BlogApp.Core.Security.Constants;
using BlogApp.Domain.Models.Blogs;

namespace BlogApp.Application.Blogs.Commands;

[Authorize(Permission.Blog.Edit)]
public record UpdateBlogCommand(
    int Id,
    string Title,
    string Content,
    string? Thumbnail,
    List<int> Categories,
    List<int> Tags) : UpdateBlogArgs(Id, Title, Content, Thumbnail, Categories, Tags),
    IRequest<Result<BlogDetailsResult>>, ICacheInvalidating
{
    public List<string> InvalidationKeys =>
    [
        $"blogs:details:{Id}",
        "blogs:top-read",
        "blogs:random"
    ];
}