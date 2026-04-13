namespace BlogApp.Core.Security.Abstractions;

public interface IRefreshTokenProvider
{
    string HashToken(string token);
    Task<string> GenerateAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<bool> ValidateAsync(string token, CancellationToken cancellationToken = default);
}