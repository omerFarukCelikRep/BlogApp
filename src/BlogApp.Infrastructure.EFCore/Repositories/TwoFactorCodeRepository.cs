using BlogApp.Domain.Abstractions.Repositories;
using BlogApp.Domain.Enums;
using BlogApp.Infrastructure.EFCore.Contexts;

namespace BlogApp.Infrastructure.EFCore.Repositories;

public class TwoFactorCodeRepository(BlogAppDbContext context)
    : EFBaseRepository<TwoFactorCode, int>(context), ITwoFactorCodeRepository
{
    public async Task RevokeAllAsync(Guid userId, TwoFactorPurpose purpose,
        CancellationToken cancellationToken = default)
    {
        await context.TwoFactorCodes
            .Where(t => t.UserId == userId && t.Purpose == purpose && !t.IsUsed)
            .ExecuteUpdateAsync(s => s.SetProperty(t => t.IsUsed, true), cancellationToken);
    }
}