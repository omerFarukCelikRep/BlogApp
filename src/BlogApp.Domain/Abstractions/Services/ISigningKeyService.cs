namespace BlogApp.Domain.Abstractions.Services;

public interface ISigningKeyService
{
    Task RotateKeysAsync(CancellationToken cancellationToken = default);
    Task<SigningKey?> GetByKeyIdAsync(string kid, CancellationToken cancellationToken = default);
    Task<SigningKey> GetActiveKey(CancellationToken cancellationToken = default);
}