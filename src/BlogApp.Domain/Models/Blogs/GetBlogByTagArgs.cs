namespace BlogApp.Domain.Models.Blogs;

public record GetBlogByTagArgs(
    string Slug,
    int? Page = 1,
    int? PageSize = 10,
    string? SortBy = "latest",
    int? TagId = null);