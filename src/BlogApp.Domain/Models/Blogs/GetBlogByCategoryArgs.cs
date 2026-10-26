namespace BlogApp.Domain.Models.Blogs;

public record GetBlogByCategoryArgs(
    string Category,
    int? Page = 1,
    int? PageSize = 10,
    string? SortBy = "latest",
    bool IncludeChildren = false,
    List<int>? Categories = null);