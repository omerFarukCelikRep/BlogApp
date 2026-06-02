using System.Security.Claims;
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
    public async Task<Result<RefreshTokenResult>> RefreshTokenAsync(RefreshTokenArgs args,
        CancellationToken cancellationToken = default)
    {
        var hashedToken = refreshTokenProvider.HashToken(args.Token);

        var storedRefreshToken = await refreshTokenRepository.GetByTokenAsync(hashedToken, cancellationToken);
        if (storedRefreshToken is null || storedRefreshToken.IsRevoked || storedRefreshToken.ExpiresAt < DateTime.Now)
            return Result<RefreshTokenResult>.Failed(null, string.Empty, 401);

        var user = storedRefreshToken.User;
        storedRefreshToken.IsRevoked = true;
        storedRefreshToken.RevokedAt = DateTime.Now;

        var newRefreshToken = await refreshTokenProvider.GenerateAsync(user!.Id, cancellationToken);
        var newJwtToken = await jwtProvider.GenerateTokenAsync(new()
        {
            UserId = user.Id,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Email = user.Email,
            Username = user.Username,
            Roles = [..user.UserRoles.Select(x => x.Role!.Name)],
            Permissions =
                [..user.UserRoles.SelectMany(x => x.Role!.RolePermissions.Select(p => p.Permission!.ToString()))]
        }, cancellationToken);

        var result = new RefreshTokenResult()
        {
            Token = newJwtToken,
            RefreshToken = newRefreshToken
        };
        return Result<RefreshTokenResult>.Success(result, "Success"); //TODO: Resource magic string
    }

    public async Task<bool> IsValidAsync(string refreshToken, CancellationToken cancellationToken = default)
    {
        var token = await refreshTokenRepository.GetByTokenAsync(refreshToken, cancellationToken);
        return token is { IsUsed: false, IsRevoked: false } && token.ExpiresAt > DateTime.UtcNow;
    }

    public async Task<ClaimsIdentity?> GetClaimsFromRefreshTokenAsync(string refreshToken,
        CancellationToken cancellationToken = default)
    {
        var token = await refreshTokenRepository.GetByTokenAsync(refreshToken, cancellationToken);
        if (token == null || token.IsUsed || token.IsRevoked || token.ExpiresAt <= DateTime.UtcNow)
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

        claims.AddRange(user.UserRoles.Select(x => new Claim(ClaimTypes.Role, x.Role!.ToString())));

        return new(claims, nameof(RefreshToken));
    }
}