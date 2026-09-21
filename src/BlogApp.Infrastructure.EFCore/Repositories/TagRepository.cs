using System.Linq.Expressions;
using BlogApp.Domain.Abstractions.Repositories;
using BlogApp.Infrastructure.EFCore.Contexts;

namespace BlogApp.Infrastructure.EFCore.Repositories;

public class TagRepository(BlogAppDbContext context) : EFBaseRepository<Tag, int>(context), ITagRepository
{
    public async Task<List<Tag>> GetTopAsync(int count, CancellationToken cancellationToken = default)
    {
        return await GetAll(false)
            .OrderByDescending(x => x.BlogTags.Count)
            .Take(count)
            .Include(x => x.BlogTags)
            .ToListAsync(cancellationToken);
    }
}