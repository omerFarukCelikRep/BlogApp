using BlogApp.Domain.Abstractions.Repositories;
using BlogApp.Infrastructure.EFCore.Contexts;

namespace BlogApp.Infrastructure.EFCore.Repositories;

public class SigningKeyRepository(BlogAppDbContext context)
    : EFBaseRepository<SigningKey, int>(context), ISigningKeyRepository
{
}