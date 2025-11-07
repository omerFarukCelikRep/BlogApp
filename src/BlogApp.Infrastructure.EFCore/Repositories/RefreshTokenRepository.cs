using BlogApp.Domain.Abstractions.Repositories;
using BlogApp.Infrastructure.EFCore.Contexts;

namespace BlogApp.Infrastructure.EFCore.Repositories;

public class RefreshTokenRepository(BlogAppDbContext context)
    : EFBaseRepository<RefreshToken, int>(context), IRefreshTokenRepository
{
    public async Task<RefreshToken?> GetByTokenAsync(string token, CancellationToken cancellationToken = default)
    {
        return await GetAsync(rt => rt.Token == token, false, cancellationToken);
    }

    public async Task RevokeAllAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var tokens = GetAll()
            .Where(rt => rt.UserId == userId && !rt.IsRevoked && !rt.IsUsed)
            .ExecuteUpdateAsync(x => x.SetProperty(a => a.IsRevoked, true), cancellationToken);

        await SaveChangesAsync(cancellationToken);
    }
}