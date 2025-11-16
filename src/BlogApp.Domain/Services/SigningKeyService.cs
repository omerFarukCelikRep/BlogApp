using System.Security.Cryptography;
using BlogApp.Domain.Abstractions.Repositories;
using BlogApp.Domain.Abstractions.Services;
using BlogApp.Domain.Options;
using Microsoft.Extensions.Options;

namespace BlogApp.Domain.Services;

public class SigningKeyService(ISigningKeyRepository signingKeyRepository, IOptions<KeyRotationOptions> options)
    : ISigningKeyService
{
    private const int KeySizeInBits = 2048;

    private readonly KeyRotationOptions _keyRotationOptions = options.Value;

    public async Task RotateKeysAsync(CancellationToken cancellationToken = default)
    {
        var activeKey = await signingKeyRepository.GetAsync(x => x.IsActive, true, cancellationToken);
        if (activeKey is null || activeKey.ExpireDate < DateTime.Now)
        {
            if (activeKey is not null)
            {
                activeKey.IsActive = false;
                await signingKeyRepository.UpdateAsync(activeKey, cancellationToken);
            }

            using var rsa = RSA.Create(KeySizeInBits);
            var privateKey = Convert.ToBase64String(rsa.ExportRSAPrivateKey());
            var publicKey = Convert.ToBase64String(rsa.ExportRSAPublicKey());
            var newKeyId = Guid.CreateVersion7().ToString();
            var newKey = new SigningKey
            {
                KeyId = newKeyId,
                PrivateKey = privateKey,
                PublicKey = publicKey,
                IsActive = true,
                ExpireDate = DateTime.Now.Add(_keyRotationOptions.Period)
            };

            await signingKeyRepository.AddAsync(newKey, cancellationToken);
            await signingKeyRepository.SaveChangesAsync(cancellationToken);
        }
    }
}