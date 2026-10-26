using BlogApp.Domain.Abstractions.Repositories;
using BlogApp.Domain.Models.Categories;
using BlogApp.Infrastructure.EFCore.Contexts;

namespace BlogApp.Infrastructure.EFCore.Repositories;

public class CategoryRepository(BlogAppDbContext context)
    : EFBaseRepository<Category, int>(context), ICategoryRepository
{
    public async Task<List<Category>> GetTopAsync(int count, CancellationToken cancellationToken = default)
    {
        return await GetAll(false)
            .OrderByDescending(x => x.BlogCategories.Count)
            .Take(count)
            .Include(x => x.BlogCategories)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<Category>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await GetAll(false)
            .Include(c => c.SubCategories)
            .Include(c => c.BlogCategories)
            .ThenInclude(c => c.Blog)
            .ToListAsync(cancellationToken);
    }

    public async Task<Category?> GetBySlugAsync(GetCategoryBySlugArgs args,
        CancellationToken cancellationToken = default)
    {
        return await GetAll(false)
            .Where(c => c.Slug.Equals(args.Slug))
            .Include(c => c.Parent)
            .Include(c => c.SubCategories)
            .ThenInclude(c => c.BlogCategories)
            .ThenInclude(c => c.Blog)
            .Include(c => c.BlogCategories)
            .ThenInclude(c => c.Blog)
            .FirstOrDefaultAsync(cancellationToken);
    }
}