namespace BlogApp.Domain.Entities;

public class Comment : BaseEntity
{
    public string Content { get; set; } = string.Empty;

    public int BlogId { get; set; }
    public Blog? Blog { get; set; }

    public Guid UserId { get; set; }
    public User? User { get; set; }
}