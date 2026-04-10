namespace BlogApp.Domain.Models.Auth;

public record LoginResult(string Token, string RefreshToken, DateTimeOffset ExpiresAt, string TokenType = "Bearer");