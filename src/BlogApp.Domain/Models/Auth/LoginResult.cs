using BlogApp.Core.Security.Constants;

namespace BlogApp.Domain.Models.Auth;

public record LoginResult(
    string Token,
    string RefreshToken,
    DateTimeOffset ExpireDate,
    string Scope = TokenScope.FullAccess,
    bool TwoFactorEnabled = false,
    Guid? UserId = null,
    string TokenType = "Bearer");