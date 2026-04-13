using BlogApp.Domain.Abstractions.Repositories;
using BlogApp.Infrastructure.EFCore.Contexts;

namespace BlogApp.Infrastructure.EFCore.Repositories;

public class SigningKeyRepository(BlogAppDbContext context)
    : EFBaseRepository<SigningKey, int>(context), ISigningKeyRepository
{
    public async Task<SigningKey?> GetByKeyIdAsync(string kid, CancellationToken cancellationToken = default)
    {
        return await GetAsync(x=> x.KeyId.Equals(kid),false,cancellationToken);
    }
}