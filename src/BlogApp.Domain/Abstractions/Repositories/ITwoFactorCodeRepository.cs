using BlogApp.Core.DataAccess.Repositories;

namespace BlogApp.Domain.Abstractions.Repositories;

public interface ITwoFactorCodeRepository : IAsyncInsertableRepository<TwoFactorCode, int>,
    IAsyncFindableRepository<TwoFactorCode, int>, IAsyncRepository
{
    Task RevokeAllAsync(Guid userId, TwoFactorPurpose purpose, CancellationToken cancellationToken = default);
}