namespace BlogApp.Domain.Entities;

public class TwoFactorCode : BaseEntity
{
    public string Code { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public DateTime ExpireDate { get; set; }
    public bool IsUsed { get; set; }
    public TwoFactorPurpose Purpose { get; set; }

    public Guid UserId { get; set; }
    public virtual User User { get; set; } = null!;

    public bool IsExpired() => DateTime.UtcNow > ExpireDate;
    public bool IsValid() => !IsUsed && !IsExpired();
}