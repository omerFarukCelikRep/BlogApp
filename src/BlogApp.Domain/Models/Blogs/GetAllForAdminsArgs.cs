namespace BlogApp.Domain.Models.Blogs;

public record GetAllForAdminsArgs(
    string? Search = null,
    PostStatus? Status = null,
    string? AuthorName = null,
    int? Page = 1,
    int? PageSize = 20);