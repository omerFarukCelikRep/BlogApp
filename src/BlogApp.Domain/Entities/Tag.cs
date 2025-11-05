namespace BlogApp.Domain.Entities;

public class Tag : BaseEntity
{
    public string Name { get; set; } = string.Empty;

    public ICollection<BlogTag> BlogTags { get; set; } = new HashSet<BlogTag>();
}