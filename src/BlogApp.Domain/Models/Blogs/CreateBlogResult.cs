namespace BlogApp.Domain.Models.Blogs;

public record CreateBlogResult(int Id, string Title, string Slug, int ReadingTimeInMinutes)
{
    public static explicit operator CreateBlogResult(Blog blog) => new(
        blog.Id,
        blog.Title,
        blog.Slug,
        blog.ReadingTimeInMinutes);
}