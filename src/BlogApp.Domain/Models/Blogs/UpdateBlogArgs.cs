namespace BlogApp.Domain.Models.Blogs;

public record UpdateBlogArgs(
    int Id,
    string Title,
    string Content,
    string? Thumbnail,
    List<int> Categories,
    List<int> Tags);