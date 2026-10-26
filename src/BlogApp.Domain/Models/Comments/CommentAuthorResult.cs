namespace BlogApp.Domain.Models.Comments;

public record CommentAuthorResult(
    Guid Id,
    string FullName,
    string Username,
    string? ProfilePicture);