namespace BlogApp.Domain.Entities;

public class Category : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? Thumbnail { get; set; }

    public ICollection<BlogCategory> BlogCategories { get; set; } = new HashSet<BlogCategory>();
}