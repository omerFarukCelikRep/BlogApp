using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using BlogApp.Domain.Abstractions.Repositories;
using BlogApp.Domain.Abstractions.Services;

namespace BlogApp.Domain.Services;

public class RefreshTokenService(IRefreshTokenRepository refreshTokenRepository, IUserRepository userRepository)
    : IRefreshTokenService
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
            ExpireDate = DateTime.UtcNow.AddDays(7),
            IsRevoked = false
        };

        await refreshTokenRepository.AddAsync(refreshToken, cancellationToken);
        await refreshTokenRepository.SaveChangesAsync(cancellationToken);
        return hashedToken;
    }

    public async Task<bool> IsValidAsync(string refreshToken, CancellationToken cancellationToken = default)
    {
        var token = await refreshTokenRepository.GetByTokenAsync(refreshToken, cancellationToken);
        return token is { IsUsed: false, IsRevoked: false } && token.ExpireDate > DateTime.UtcNow;
    }

    public async Task<ClaimsIdentity?> GetClaimsFromRefreshTokenAsync(string refreshToken,
        CancellationToken cancellationToken = default)
    {
        var token = await refreshTokenRepository.GetByTokenAsync(refreshToken, cancellationToken);
        if (token == null || token.IsUsed || token.IsRevoked || token.ExpireDate <= DateTime.UtcNow)
            return null;

        var user = await userRepository.GetByIdAsync(token.UserId, false, cancellationToken);
        if (user is null)
            return null;

        token.IsUsed = true;
        await refreshTokenRepository.UpdateAsync(token, cancellationToken);

        List<Claim> claims =
        [
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Name, user.Username)
        ];

        claims.AddRange(user.Roles.Select(x => new Claim(ClaimTypes.Role, x.Role!.ToString())));

        return new(claims, nameof(RefreshToken));
    }
}