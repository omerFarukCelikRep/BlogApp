namespace BlogApp.Domain.Models.Blogs;

public record BlogSummaryResult(
    int Id,
    string Title,
    string Excerpt,
    string AuthorFullName,
    string Tag,
    int ReadingTimeInMinutes,
    int ReadCount,
    string? Slug,
    string? Thumbnail,
    DateTime PublishDate)
{
    public static explicit operator BlogSummaryResult(Blog blog)
    {
        return new(
            Id: blog.Id,
            Title: blog.Title,
            Excerpt: blog.Content[..Math.Min(120, blog.Content.Length)] + "...",
            AuthorFullName: blog.Author?.FullName ?? "Unknown",
            Tag: blog.BlogTags.FirstOrDefault()?.Tag?.Name ?? "General",
            ReadingTimeInMinutes: blog.ReadingTimeInMinutes,
            ReadCount: blog.ReadCount,
            Slug: blog.Slug,
            Thumbnail: blog.Thumbnail,
            PublishDate: blog.CreatedDate.DateTime);
    }
}