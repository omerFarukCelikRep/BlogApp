namespace BlogApp.Domain.Entities;

public class Category : BaseEntity
{
    public required string Name { get; set; }
    public string? Description { get; set; }
    public string? Thumbnail { get; set; }
    public required string Slug { get; set; }

    public int? ParentId { get; set; }
    public virtual Category? Parent { get; set; }

    public virtual ICollection<Category> SubCategories { get; set; } = new HashSet<Category>();
    public virtual ICollection<BlogCategory> BlogCategories { get; set; } = new HashSet<BlogCategory>();
}