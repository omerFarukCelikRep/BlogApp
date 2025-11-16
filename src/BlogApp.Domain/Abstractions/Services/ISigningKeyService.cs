namespace BlogApp.Domain.Abstractions.Services;

public interface ISigningKeyService
{
    Task RotateKeysAsync(CancellationToken cancellationToken = default);
}