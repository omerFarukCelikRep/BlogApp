namespace BlogApp.Core.Security.Abstractions;

public interface IRefreshTokenProvider
{
    Task<string> GenerateRefreshTokenAsync(Guid userId, CancellationToken cancellationToken = default);
}