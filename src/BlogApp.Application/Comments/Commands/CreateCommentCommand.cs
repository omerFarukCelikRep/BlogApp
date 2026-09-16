using BlogApp.Core.Caching.Markers;
using BlogApp.Core.Security.Attributes;
using BlogApp.Core.Security.Constants;
using BlogApp.Domain.Models.Comments;

namespace BlogApp.Application.Comments.Commands;

[Authorize(Permission.Comment.Create)]
public record CreateCommentCommand(int BlogId, string Content, int? ParentId = null)
    : CreateCommentArgs(BlogId, Content, ParentId), IRequest<Result<CommentResult>>, ICacheInvalidating
{
    public List<string> InvalidationKeys => [$"comments:blog:{BlogId}"];
}