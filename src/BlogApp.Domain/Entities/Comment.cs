namespace BlogApp.Domain.Entities;

public class Comment : BaseEntity
{
    public required string Content { get; set; }

    public int BlogId { get; set; }
    public virtual Blog? Blog { get; set; }

    public Guid UserId { get; set; }
    public virtual User? User { get; set; }
}