namespace BlogApp.Domain.Entities;

public class Comment : BaseEntity
{
    public required string Content { get; set; }

    public int BlogId { get; set; }
    public virtual Blog? Blog { get; set; }

    public Guid UserId { get; set; }
    public virtual User? User { get; set; }

    public int ParentId { get; set; }
    public virtual Comment? Parent { get; set; }

    public virtual ICollection<Comment> Replies { get; set; } = new HashSet<Comment>();

    public bool IsOwnedBy(Guid userId) => UserId == userId;
}