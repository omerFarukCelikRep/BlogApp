namespace BlogApp.Domain.Entities;

public class Blog : SoftDeletableEntity
{
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string? Thumbnail { get; set; }
    public int ReadingTimeInMinutes { get; set; }
    public PostStatus PostStatus { get; set; }

    public Guid AuthorId { get; set; }
    public User? Author { get; set; }

    public ICollection<Comment> Comments { get; set; } = new HashSet<Comment>();
    public ICollection<BlogCategory> BlogCategories { get; set; } = new HashSet<BlogCategory>();
    public ICollection<Like> Likes { get; set; } = new HashSet<Like>();
    public ICollection<BlogTag> BlogTags { get; set; } = new HashSet<BlogTag>();
}