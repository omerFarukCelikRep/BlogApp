namespace BlogApp.Domain.Entities;

public class PasswordResetToken : BaseEntity
{
    public required string Token { get; set; }
    public required string OtpCode { get; set; }
    public DateTime ExpireDate { get; set; }
    public bool IsUsed { get; set; }

    public Guid UserId { get; set; }
    public virtual User User { get; set; } = null!;

    public bool IsExpired() => DateTime.UtcNow > ExpireDate;
    public bool IsValid() => !IsUsed && !IsExpired();
}