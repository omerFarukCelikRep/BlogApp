namespace BlogApp.Domain.Models.Blogs;

public record BlogForUpdateResult(
    int Id,
    string Title,
    string Content,
    string? Thumbnail,
    int ReadingTimeInMinutes,
    List<int> Categories,
    List<int> Tags)
{
    public static explicit operator BlogForUpdateResult(Blog blog) => new(
        blog.Id,
        blog.Title,
        blog.Content,
        blog.Thumbnail,
        blog.ReadingTimeInMinutes,
        [.. blog.BlogCategories.Select(x => x.CategoryId)],
        [.. blog.BlogTags.Select(x => x.TagId)]);
}