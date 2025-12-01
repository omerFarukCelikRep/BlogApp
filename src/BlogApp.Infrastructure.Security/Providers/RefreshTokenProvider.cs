using System.Security.Cryptography;
using System.Text;
using BlogApp.Core.Security.Abstractions;
using BlogApp.Domain.Abstractions.Repositories;
using BlogApp.Domain.Entities;

namespace BlogApp.Infrastructure.Security.Providers;

public class RefreshTokenProvider(IRefreshTokenRepository refreshTokenRepository) : IRefreshTokenProvider
{
    public async Task<string> GenerateRefreshTokenAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        using var randomNumberGenerator = RandomNumberGenerator.Create();
        var randomNumber = new byte[64];
        randomNumberGenerator.GetBytes(randomNumber);

        var generatedRefreshToken = Convert.ToBase64String(randomNumber);
        var hashedTokenBytes = SHA512.HashData(Encoding.UTF8.GetBytes(generatedRefreshToken));
        var hashedToken = Convert.ToBase64String(hashedTokenBytes);

        RefreshToken refreshToken = new()
        {
            Token = generatedRefreshToken,
            UserId = userId,
            ExpireDate = DateTime.Now.AddDays(7),
            IsRevoked = false
        };

        await refreshTokenRepository.AddAsync(refreshToken, cancellationToken);
        await refreshTokenRepository.SaveChangesAsync(cancellationToken);
        return hashedToken;
    }
}