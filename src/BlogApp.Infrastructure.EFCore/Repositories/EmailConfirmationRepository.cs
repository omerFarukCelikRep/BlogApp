using BlogApp.Domain.Abstractions.Repositories;
using BlogApp.Infrastructure.EFCore.Contexts;

namespace BlogApp.Infrastructure.EFCore.Repositories;

public class EmailConfirmationRepository(BlogAppDbContext context)
    : EFBaseRepository<EmailConfirmation, int>(context), IEmailConfirmationRepository
{
    public async Task RevokeAllForUserAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        await context.EmailConfirmations
            .Where(e => e.UserId == userId && !e.IsUsed)
            .ExecuteUpdateAsync(s => s.SetProperty(t => t.IsUsed, true), cancellationToken);
    }
}