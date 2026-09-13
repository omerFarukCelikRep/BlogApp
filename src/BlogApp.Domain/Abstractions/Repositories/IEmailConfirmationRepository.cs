using BlogApp.Core.DataAccess.Repositories;

namespace BlogApp.Domain.Abstractions.Repositories;

public interface IEmailConfirmationRepository : IAsyncInsertableRepository<EmailConfirmation, int>,
    IAsyncFindableRepository<EmailConfirmation, int>, IAsyncRepository
{
    Task RevokeAllForUserAsync(Guid userId, CancellationToken cancellationToken = default);
}