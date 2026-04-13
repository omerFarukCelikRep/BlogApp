using BlogApp.Domain.Abstractions.Repositories;
using BlogApp.Infrastructure.EFCore.Contexts;

namespace BlogApp.Infrastructure.EFCore.Repositories;

public class RefreshTokenRepository(BlogAppDbContext context)
    : EFBaseRepository<RefreshToken, int>(context), IRefreshTokenRepository
{
    public async Task<RefreshToken?> GetByTokenAsync(string token, CancellationToken cancellationToken = default)
    {
        return await GetAsync(rt => rt.Token == token, true, cancellationToken);
    }

    public async Task RevokeAsync(string token, string? revokedIp = null, CancellationToken cancellationToken = default)
    {
        var refreshToken = await GetByTokenAsync(token, cancellationToken);
        if (refreshToken is null)
            return;

        refreshToken.IsRevoked = true;
        refreshToken.RevokedAt = DateTimeOffset.Now;
        refreshToken.RevokedIp = revokedIp;

        await UpdateAsync(refreshToken, cancellationToken);
    }

    public async Task RevokeAllAsync(Guid userId, string? revokedIp = null,
        CancellationToken cancellationToken = default)
    {
        await GetAll()
            .Where(rt => rt.UserId == userId && !rt.IsRevoked && !rt.IsUsed)
            .ExecuteUpdateAsync(x =>
            {
                x.SetProperty(a => a.IsRevoked, true);
                x.SetProperty(a => a.RevokedIp, revokedIp);
                x.SetProperty(a => a.RevokedAt, DateTimeOffset.Now);
            }, cancellationToken);

        await SaveChangesAsync(cancellationToken);
    }
}