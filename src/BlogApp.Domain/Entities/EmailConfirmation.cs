namespace BlogApp.Domain.Entities;

public class EmailConfirmation : BaseEntity
{
    public string Token { get; set; } = string.Empty;
    public string OtpCode { get; set; } = string.Empty;
    public DateTime ExpireDate { get; set; }
    public bool IsUsed { get; set; }

    public Guid UserId { get; set; }
    public virtual User User { get; set; } = null!;

    public bool IsExpired() => DateTime.UtcNow > ExpireDate;
    public bool IsValid() => !IsUsed && !IsExpired();
}