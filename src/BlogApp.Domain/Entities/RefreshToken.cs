namespace BlogApp.Domain.Entities;

public class RefreshToken : BaseEntity
{
    public required string Token { get; set; }
    public DateTime ExpireDate { get; set; }
    public bool IsRevoked { get; set; }
    public DateTime? RevokedDate { get; set; }
    public bool IsUsed { get; set; }
    public string? CreatedIp { get; set; }

    public Guid UserId { get; set; }
    public virtual User? User { get; set; }
}