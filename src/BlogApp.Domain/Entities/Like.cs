namespace BlogApp.Domain.Entities;

public class Like : BaseEntity
{
    public int BlogId { get; set; }
    public virtual Blog? Blog { get; set; }

    public Guid UserId { get; set; }
    public virtual User? User { get; set; }
}