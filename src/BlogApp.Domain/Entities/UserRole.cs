namespace BlogApp.Domain.Entities;

public class UserRole : BaseEntity
{
    public Guid UserId { get; set; }
    public User? User { get; set; }

    public int RoleId { get; set; }
    public Role? Role { get; set; }
}