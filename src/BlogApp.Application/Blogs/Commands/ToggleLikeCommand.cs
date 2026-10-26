using BlogApp.Core.Caching.Markers;
using BlogApp.Core.Security.Attributes;
using BlogApp.Core.Security.Enums;
using BlogApp.Domain.Models.Blogs;

namespace BlogApp.Application.Blogs.Commands;

[Authorize(Role.Reader)]
public record ToggleLikeCommand(int Id) : ToggleLikeArgs(Id), IRequest<Result<ToggleLikeResult>>, ICacheInvalidating
{
    public List<string> InvalidationKeys => [$"blogs:detail:{Id}"];
}