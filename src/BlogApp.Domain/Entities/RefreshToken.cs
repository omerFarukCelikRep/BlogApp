namespace BlogApp.Domain.Entities;

public class RefreshToken : BaseEntity
{
    public required string Token { get; set; }
    public DateTimeOffset ExpiresAt { get; set; }
    public bool IsRevoked { get; set; }
    public DateTimeOffset? RevokedAt { get; set; }
    public string? RevokedIp { get; set; }
    public bool IsUsed { get; set; }
    public string? ReplacedByToken { get; set; }
    public string? CreatedIp { get; set; }

    public Guid UserId { get; set; }
    public virtual User? User { get; set; }

    public bool IsExpired() => DateTimeOffset.Now >= ExpiresAt;
    public bool IsActive() => !IsRevoked && !IsUsed && !IsExpired();
}