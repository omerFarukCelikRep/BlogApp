namespace BlogApp.Domain.Entities;

public class Category : BaseEntity
{
    public required string Name { get; set; }
    public string? Description { get; set; }
    public string? Thumbnail { get; set; }

    public virtual ICollection<BlogCategory> BlogCategories { get; set; } = new HashSet<BlogCategory>();
}