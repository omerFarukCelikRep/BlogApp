namespace BlogApp.Domain.Models.Blogs;

public record GetBlogByAuthorArgs(
    string Username,
    int? Page = 1,
    int? PageSize = 10,
    string? SortBy = "latest",
    Guid? AuthorId = null);