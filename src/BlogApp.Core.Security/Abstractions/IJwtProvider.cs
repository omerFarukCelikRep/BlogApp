using System.Security.Claims;
using BlogApp.Core.Security.Models;

namespace BlogApp.Core.Security.Abstractions;

public interface IJwtProvider
{
    Task<string> GenerateTokenAsync(TokenArgs args, CancellationToken cancellationToken = default);
    Task<ClaimsPrincipal?> ValidateTokenAsync(string token, CancellationToken cancellationToken = default);
}