namespace BlogApp.Domain.Entities;

public class RefreshToken : BaseEntity
{
    public string Token { get; set; } = null!;
    public DateTime ExpireDate { get; set; }
    public bool IsRevoked { get; set; }
    public bool IsUsed { get; set; }
    public string? CreatedIp { get; set; }

    public Guid UserId { get; set; }
    public User? User { get; set; }
}