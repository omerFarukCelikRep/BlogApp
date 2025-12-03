namespace BlogApp.Core.Security.Abstractions;

public interface IRefreshTokenProvider
{
    string HashToken(string token);
    Task<string> GenerateRefreshTokenAsync(Guid userId, CancellationToken cancellationToken = default);
}