namespace BlogApp.Domain.Entities;

public class Tag : BaseEntity
{
    public required string Name { get; set; }
    public required string Slug { get; set; }
    public string? Description { get; set; }

    public virtual ICollection<BlogTag> BlogTags { get; set; } = new HashSet<BlogTag>();
}