namespace BlogApp.Domain.Entities;

public class Blog : SoftDeletableEntity
{
    public required string Title { get; set; }
    public required string Content { get; set; }
    public string? Thumbnail { get; set; }
    public int ReadingTimeInMinutes { get; set; }
    public PostStatus PostStatus { get; set; }
    public int ReadCount { get; private set; }
    public required string Slug { get; set; }
    public DateTime? PublishDate { get; set; }

    public Guid AuthorId { get; set; }
    public virtual User? Author { get; set; }

    public virtual ICollection<Comment> Comments { get; set; } = new HashSet<Comment>();
    public virtual ICollection<BlogCategory> BlogCategories { get; set; } = new HashSet<BlogCategory>();
    public virtual ICollection<Like> Likes { get; set; } = new HashSet<Like>();
    public virtual ICollection<BlogTag> BlogTags { get; set; } = new HashSet<BlogTag>();

    private static int CalculateReadingTime(string content) => Math.Max(1, content.Split(' ').Length / 200);

    public bool IsPublished() => PostStatus == PostStatus.Published;
    public bool IsDraft() => PostStatus == PostStatus.Draft;
    public bool IsArchived() => PostStatus == PostStatus.Archived;
    public bool IsOwnedBy(Guid authorId) => AuthorId == authorId;

    public void Publish() => PostStatus = PostStatus.Published;
    public void Archive() => PostStatus = PostStatus.Archived;
    public void Unpublish() => PostStatus = PostStatus.Draft;

    public void UpdateContent(string title, string content, string slug, string? thumbnail = null)
    {
        Title = title;
        Content = content;
        Thumbnail = thumbnail;
        Slug = slug;
        ReadingTimeInMinutes = CalculateReadingTime(content);
    }

    public void InitializeReadingTime() => ReadingTimeInMinutes = CalculateReadingTime(Content);

    public void IncrementReadCount() => ReadCount++;
}