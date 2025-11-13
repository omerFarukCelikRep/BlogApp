using BlogApp.Core.DataAccess.Repositories;

namespace BlogApp.Domain.Abstractions.Repositories;

public interface IRefreshTokenRepository : IAsyncInsertableRepository<RefreshToken, int>,
    IAsyncUpdateableRepository<RefreshToken, int>
{
    Task<RefreshToken?> GetByTokenAsync(string token, CancellationToken cancellationToken = default);
    Task RevokeAllAsync(Guid userId, CancellationToken cancellationToken = default);
}