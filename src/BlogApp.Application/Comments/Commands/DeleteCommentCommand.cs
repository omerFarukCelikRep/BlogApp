using BlogApp.Core.Caching.Markers;
using BlogApp.Core.Security.Attributes;
using BlogApp.Core.Security.Constants;
using BlogApp.Domain.Models.Comments;

namespace BlogApp.Application.Comments.Commands;

[Authorize(Permission.Comment.Create)]
public record DeleteCommentCommand(int Id, int BlogId) : DeleteCommentArgs(Id), IRequest<Result>, ICacheInvalidating
{
    public List<string> InvalidationKeys => [$"comments:blog:{BlogId}"];
}