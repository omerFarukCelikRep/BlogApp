namespace BlogApp.Domain.Entities;

public class Blog : SoftDeletableEntity
{
    public required string Title { get; set; }
    public required string Content { get; set; }
    public string? Thumbnail { get; set; }
    public int ReadingTimeInMinutes { get; set; }
    public PostStatus PostStatus { get; set; }

    public Guid AuthorId { get; set; }
    public virtual User? Author { get; set; }

    public virtual ICollection<Comment> Comments { get; set; } = new HashSet<Comment>();
    public virtual ICollection<BlogCategory> BlogCategories { get; set; } = new HashSet<BlogCategory>();
    public virtual ICollection<Like> Likes { get; set; } = new HashSet<Like>();
    public virtual ICollection<BlogTag> BlogTags { get; set; } = new HashSet<BlogTag>();
}