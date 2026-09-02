namespace BlogApp.Domain.Models.RefreshTokens;

public record RefreshTokenResult(string Token, string RefreshToken, DateTime ExpireDate);