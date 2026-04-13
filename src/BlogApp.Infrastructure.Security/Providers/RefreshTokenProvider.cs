using System.Security.Cryptography;
using System.Text;
using BlogApp.Core.Security.Abstractions;
using BlogApp.Core.Security.Options;
using BlogApp.Domain.Abstractions.Repositories;
using BlogApp.Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace BlogApp.Infrastructure.Security.Providers;

public class RefreshTokenProvider(
    IRefreshTokenRepository refreshTokenRepository,
    IHttpContextAccessor httpContextAccessor,
    IOptions<JwtOptions> options) : IRefreshTokenProvider
{
    private readonly JwtOptions _jwtOptions = options.Value;

    public string HashToken(string token)
    {
        var hashedTokenBytes = SHA512.HashData(Encoding.UTF8.GetBytes(token));
        return Convert.ToBase64String(hashedTokenBytes);
    }

    public async Task<string> GenerateAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var rawToken = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));

        var hashedToken = HashToken(rawToken);

        var ip = httpContextAccessor.HttpContext?.Connection.RemoteIpAddress?.ToString();

        RefreshToken refreshToken = new()
        {
            Token = hashedToken,
            UserId = userId,
            ExpiresAt = DateTimeOffset.Now.AddDays(_jwtOptions.RefreshTokenExpirationDays),
            IsRevoked = false,
            IsUsed = false,
            CreatedIp = ip
        };

        await refreshTokenRepository.AddAsync(refreshToken, cancellationToken);
        await refreshTokenRepository.SaveChangesAsync(cancellationToken);
        return hashedToken;
    }

    public async Task<bool> ValidateAsync(string token, CancellationToken cancellationToken = default)
    {
        var hashedToken = HashToken(token);

        var refreshToken = await refreshTokenRepository.GetByTokenAsync(hashedToken, cancellationToken);
        return refreshToken is not null && refreshToken.IsActive();
    }

    public async Task RevokeAsync(string token, string? replacedByToken = null,
        CancellationToken cancellationToken = default)
    {
        var hashedToken = HashToken(token);

        var ip = httpContextAccessor.HttpContext?.Connection.RemoteIpAddress?.ToString();
        
        await refreshTokenRepository.RevokeAsync(hashedToken, ip, cancellationToken);
        await refreshTokenRepository.SaveChangesAsync(cancellationToken);
    }

    public async Task RevokeAllAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var ip = httpContextAccessor.HttpContext?.Connection.RemoteIpAddress?.ToString();

        await refreshTokenRepository.RevokeAllAsync(userId, ip, cancellationToken);
        await refreshTokenRepository.SaveChangesAsync(cancellationToken);
    }
}