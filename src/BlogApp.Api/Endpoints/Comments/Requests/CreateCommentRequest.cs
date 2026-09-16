using BlogApp.Application.Comments.Commands;
using BlogApp.Core.Security.Attributes;

namespace BlogApp.Api.Endpoints.Comments.Requests;

public record CreateCommentRequest(
    int BlogId,
    [Sanitize] string Content,
    int? ParentId)
{
    public static explicit operator CreateCommentCommand(CreateCommentRequest request) => new(
        BlogId: request.BlogId,
        Content: request.Content,
        ParentId: request.ParentId);
}