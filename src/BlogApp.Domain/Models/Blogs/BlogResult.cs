namespace BlogApp.Domain.Models.Blogs;

public record BlogResult(
    int Id,
    string Title,
    string Content,
    string? Thumbnail,
    int ReadingTimeInMinutes,
    int ReadCount,
    string Slug,
    string Author,
    int LikesCount,
    List<BlogCategoryResult> Categories,
    List<BlogTagResult> Tags)

{
    public static explicit operator BlogResult(Blog blog) =>
        new(blog.Id,
            blog.Title,
            blog.Content,
            blog.Thumbnail,
            blog.ReadingTimeInMinutes,
            blog.ReadCount,
            blog.Slug,
            blog.Author!.FullName,
            blog.Likes.Count,
            [
                .. blog.BlogCategories.Select(x =>
                {
                    var category = x.Category;
                    return new BlogCategoryResult(category!.Id, category!.Name,category.Slug);
                })
            ],
            [
                .. blog.BlogTags.Select(x =>
                {
                    var tag = x.Tag;
                    return new BlogTagResult(tag!.Id, tag!.Name,tag.Slug);
                })
            ]);
}