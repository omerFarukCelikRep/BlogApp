using BlogApp.Domain.Abstractions.Repositories;
using BlogApp.Domain.Models.Authors;
using BlogApp.Infrastructure.EFCore.Contexts;

namespace BlogApp.Infrastructure.EFCore.Repositories;

public class UserRepository(BlogAppDbContext context) : EFBaseRepository<User, Guid>(context), IUserRepository
{
    public async Task<User?> GetAuthorProfileAsync(GetByUsernameArgs args,
        CancellationToken cancellationToken = default)
    {
        return await GetAll(false)
            .Where(x => x.Username.Equals(args.Username))
            .Include(x => x.Blogs.Where(b => b.IsPublished()))
            .ThenInclude(x => x.BlogTags)
            .ThenInclude(x => x.Tag)
            .Include(x => x.Blogs)
            .ThenInclude(x => x.Likes)
            .FirstOrDefaultAsync(cancellationToken);
    }
}