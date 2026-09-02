namespace BlogApp.Domain.Entities;

public class SigningKey : SoftDeletableEntity
{
    public required string KeyId { get; set; }
    public required string PrivateKey { get; set; }
    public required string PublicKey { get; set; }
    public bool IsActive { get; set; }
    public DateTime ExpireDate { get; set; }
}