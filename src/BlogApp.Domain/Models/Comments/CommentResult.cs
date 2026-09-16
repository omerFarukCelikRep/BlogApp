using BlogApp.Core.DataAccess.Enums;

namespace BlogApp.Domain.Models.Comments;

public record CommentResult(
    int Id,
    string Content,
    bool IsEdited,
    DateTime CreatedDate,
    CommentAuthorResult Author,
    int? ParentId,
    List<CommentResult> Replies)
{
    public static explicit operator CommentResult(Comment comment) => new(
        Id: comment.Id,
        Content: comment.Content,
        IsEdited: comment.Status == Status.Modified,
        CreatedDate: comment.CreatedDate.DateTime,
        Author: new(comment.User!.Id, comment.User.FullName, comment.User.Username, comment.User.ProfilePicture),
        ParentId: comment.ParentId,
        Replies: [.. comment.Replies.Select(x => (CommentResult)x)]);
}