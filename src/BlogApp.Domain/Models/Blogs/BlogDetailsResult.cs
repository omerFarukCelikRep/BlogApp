namespace BlogApp.Domain.Models.Blogs;

public record BlogDetailsResult(
    int Id,
    string Title,
    string Content,
    string Slug,
    string? Thumbnail,
    int ReadingTimeInMinutes,
    int ReadCount,
    int LikeCount,
    bool IsLiked,
    PostStatus PostStatus,
    DateTime? PublishDate,
    BlogAuthorResult Author,
    List<BlogTagResult> Tags,
    List<BlogCategoryResult> Categories)
{
    public static explicit operator BlogDetailsResult(Blog blog) => new(
        Id: blog.Id,
        Title: blog.Title,
        Content: blog.Content,
        Slug: blog.Slug,
        Thumbnail: blog.Thumbnail,
        ReadingTimeInMinutes: blog.ReadingTimeInMinutes,
        ReadCount: blog.ReadCount,
        LikeCount: blog.Likes.Count,
        IsLiked: false,
        PostStatus: blog.PostStatus,
        PublishDate: blog.PublishDate,
        Author: new BlogAuthorResult(
            Id: blog.Author!.Id,
            FullName: blog.Author.FullName,
            Username: blog.Author.Username,
            Bio: blog.Author.Bio,
            ProfilePicture: blog.Author.ProfilePicture),
        Tags:
        [
            .. blog.BlogTags.Select(bt => new BlogTagResult(
                Id: bt.Tag!.Id,
                Name: bt.Tag.Name,
                Slug: bt.Tag.Slug))
        ],
        Categories:
        [
            .. blog.BlogCategories.Select(bc => new BlogCategoryResult(
                Id: bc.Category!.Id,
                Name: bc.Category.Name,
                Slug: bc.Category.Slug))
        ]);
}