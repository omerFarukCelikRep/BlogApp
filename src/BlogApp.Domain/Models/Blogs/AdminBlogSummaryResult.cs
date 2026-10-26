namespace BlogApp.Domain.Models.Blogs;

public record AdminBlogSummaryResult(
    int Id,
    string Title,
    string Slug,
    string AuthorFullName,
    string AuthorUsername,
    string PostStatus,
    int ReadCount,
    int LikeCount,
    int CommentCount,
    DateTime CreatedDate,
    List<string> Categories,
    List<string> Tags)
{
    public static explicit operator AdminBlogSummaryResult(Blog blog) => new AdminBlogSummaryResult(
        Id: blog.Id,
        Title: blog.Title,
        Slug: blog.Slug,
        AuthorFullName: blog.Author!.FullName,
        AuthorUsername: blog.Author.Username,
        PostStatus: blog.PostStatus.ToString(),
        ReadCount: blog.ReadCount,
        LikeCount: blog.Likes.Count,
        CommentCount: blog.Comments.Count,
        CreatedDate: blog.CreatedDate.DateTime,
        Categories: [.. blog.BlogCategories.Select(bt => bt.Category!.Name)],
        Tags: [.. blog.BlogTags.Select(bt => bt.Tag!.Name)]);
}