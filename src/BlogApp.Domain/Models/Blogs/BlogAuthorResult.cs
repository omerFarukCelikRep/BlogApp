namespace BlogApp.Domain.Models.Blogs;

public record BlogAuthorResult(
    Guid   Id,
    string FullName,
    string Username,
    string? Bio,
    string? ProfilePicture);