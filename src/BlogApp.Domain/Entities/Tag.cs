namespace BlogApp.Domain.Entities;

public class Tag : BaseEntity
{
    public required string Name { get; set; }

    public virtual ICollection<BlogTag> BlogTags { get; set; } = new HashSet<BlogTag>();
}