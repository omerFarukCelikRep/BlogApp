using BlogApp.Core.Caching.Markers;
using BlogApp.Core.Security.Attributes;
using BlogApp.Core.Security.Constants;
using BlogApp.Domain.Models.Blogs;

namespace BlogApp.Application.Blogs.Commands;

[Authorize(Permission.Blog.Create)]
public record CreateBlogCommand(
    string Title,
    string Content,
    string? Thumbnail,
    List<int> Categories,
    List<int> Tags
) : CreateBlogArgs(Title, Content, Thumbnail, Categories, Tags), IRequest<Result<CreateBlogResult>>, ICacheInvalidating
{
    public List<string> InvalidationKeys => ["blogs:top-read", "blogs:random"];
}