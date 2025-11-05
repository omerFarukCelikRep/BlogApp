using System.Security.Claims;
using BlogApp.Core.Security.Models;

namespace BlogApp.Core.Security.Abstractions;

public interface IJwtProvider
{
    Task<string> GenerateTokenAsync(Guid userId, CancellationToken cancellationToken = default);
    ClaimsPrincipal? ValidateToken(string token);
}