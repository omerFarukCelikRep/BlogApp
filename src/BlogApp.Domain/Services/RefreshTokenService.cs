using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using BlogApp.Core.Results;
using BlogApp.Core.Security.Abstractions;
using BlogApp.Domain.Abstractions.Repositories;
using BlogApp.Domain.Abstractions.Services;
using BlogApp.Domain.Models.RefreshTokens;

namespace BlogApp.Domain.Services;

public class RefreshTokenService(
    IRefreshTokenRepository refreshTokenRepository,
    IUserRepository userRepository,
    IJwtProvider jwtProvider,
    IRefreshTokenProvider refreshTokenProvider)
    : IRefreshTokenService
{
    public async Task<Result<RefreshTokenResult>> RefreshTokenAsync(string refreshToken,
        CancellationToken cancellationToken = default)
    {
        var hashedBytes = SHA512.HashData(Encoding.UTF8.GetBytes(refreshToken));
        var hashedToken = Convert.ToBase64String(hashedBytes);

        var storedRefreshToken = await refreshTokenRepository.GetByTokenAsync(hashedToken, cancellationToken);
        if (storedRefreshToken is null || storedRefreshToken.IsRevoked || storedRefreshToken.ExpireDate < DateTime.Now)
            return Result<RefreshTokenResult>.Failed(null, string.Empty, 401);

        var user = storedRefreshToken.User;
        storedRefreshToken.IsRevoked = true;
        storedRefreshToken.RevokedDate = DateTime.Now;

        var newRefreshToken = await refreshTokenProvider.GenerateRefreshTokenAsync(user!.Id, cancellationToken);
        var newJwtToken = await jwtProvider.GenerateTokenAsync(user.Id, cancellationToken);

        var result = new RefreshTokenResult()
        {
            Token = newJwtToken,
            RefreshToken = newRefreshToken
        };
        return Result<RefreshTokenResult>.Success(result, "Success", 200); //TODO: Resource magic string
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