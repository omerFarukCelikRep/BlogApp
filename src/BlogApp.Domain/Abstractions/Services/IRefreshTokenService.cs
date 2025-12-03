using System.Security.Claims;
using BlogApp.Core.Results;
using BlogApp.Domain.Models.RefreshTokens;

namespace BlogApp.Domain.Abstractions.Services;

public interface IRefreshTokenService
{
    Task<Result<RefreshTokenResult>> RefreshTokenAsync(RefreshTokenArgs args,
        CancellationToken cancellationToken = default);

    Task<bool> IsValidAsync(string refreshToken, CancellationToken cancellationToken = default);

    Task<ClaimsIdentity?> GetClaimsFromRefreshTokenAsync(string refreshToken,
        CancellationToken cancellationToken = default);
}