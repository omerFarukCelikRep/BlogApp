namespace BlogApp.Domain.Entities;

public class Like : BaseEntity
{
    public int BlogId { get; set; }
    public Blog? Blog { get; set; }

    public Guid UserId { get; set; }
    public User? User { get; set; }
}