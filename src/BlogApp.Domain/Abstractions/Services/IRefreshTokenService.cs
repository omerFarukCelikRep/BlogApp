using System.Security.Claims;

namespace BlogApp.Domain.Abstractions.Services;

public interface IRefreshTokenService
{
    Task<bool> IsValidAsync(string refreshToken, CancellationToken cancellationToken = default);

    Task<ClaimsIdentity?> GetClaimsFromRefreshTokenAsync(string refreshToken,
        CancellationToken cancellationToken = default);
}