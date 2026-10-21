namespace BlogApp.Domain.Models.Blogs;

public record CreateBlogArgs(
    string Title,
    string Content,
    string? Thumbnail,
    List<int> Categories,
    List<int> Tags);