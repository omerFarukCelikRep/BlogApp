using BlogApp.Core.DataAccess.Repositories;

namespace BlogApp.Domain.Abstractions.Repositories;

public interface IPasswordResetTokenRepository : IAsyncFindableRepository<PasswordResetToken, int>,
    IAsyncInsertableRepository<PasswordResetToken, int>, IAsyncRepository
{
    Task RevokeAllForUserAsync(Guid userId, CancellationToken cancellationToken = default);
}