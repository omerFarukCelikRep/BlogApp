namespace BlogApp.Domain.Entities;

public class User : SoftDeletableEntity<Guid>
{
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public required string Username { get; set; }
    public required string Email { get; set; }
    public bool EmailConfirmed { get; set; }
    public required string Password { get; set; }
    public bool TwoFactorEnabled { get; set; }
    public DateTimeOffset? LockoutEnd { get; set; }
    public int AccessFailedCount { get; set; }

    public virtual ICollection<Blog> Blogs { get; set; } = new HashSet<Blog>();
    public virtual ICollection<Like> Likes { get; set; } = new HashSet<Like>();
    public virtual ICollection<UserRole> Roles { get; set; } = new HashSet<UserRole>();
    public virtual ICollection<RefreshToken> RefreshTokens { get; set; } = new HashSet<RefreshToken>();
}