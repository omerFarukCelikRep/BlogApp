namespace BlogApp.Domain.Entities;

public class BlogCategory : BaseEntity
{
    public int BlogId { get; set; }
    public virtual Blog? Blog { get; set; }

    public int CategoryId { get; set; }
    public virtual Category? Category { get; set; }
}