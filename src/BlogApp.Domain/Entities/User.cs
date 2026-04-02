namespace BlogApp.Domain.Entities;

public class User : SoftDeletableEntity<Guid>
{
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public string FullName => $"{FirstName} {LastName}";
    public required string Username { get; set; }
    public required string Email { get; set; }
    public bool EmailConfirmed { get; set; }
    public required string Password { get; set; }
    public string? Bio { get; set; }
    public string? ProfilePicture { get; set; }
    public bool TwoFactorEnabled { get; set; }
    public DateTimeOffset? LockoutEnd { get; set; }
    public int AccessFailedCount { get; set; }

    public virtual ICollection<Blog> Blogs { get; set; } = new HashSet<Blog>();
    public virtual ICollection<Like> Likes { get; set; } = new HashSet<Like>();
    public virtual ICollection<UserRole> UserRoles { get; set; } = new HashSet<UserRole>();
    public virtual ICollection<RefreshToken> RefreshTokens { get; set; } = new HashSet<RefreshToken>();
    public virtual ICollection<Comment> Comments { get; set; } = new HashSet<Comment>();
    
    public bool IsLockedOut() => LockoutEnd.HasValue &&  LockoutEnd.Value <= DateTimeOffset.Now;
    
    public bool IsEmailConfirmed() => EmailConfirmed;
    
    public bool ConfirmEmail() => EmailConfirmed = true;
    
    public void EnableTwoFactorAuthentication() => TwoFactorEnabled = true;
    
    public void DisableTwoFactorAuthentication() => TwoFactorEnabled = false;
}