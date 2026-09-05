using BlogApp.Domain.Abstractions.Repositories;
using BlogApp.Domain.Enums;
using BlogApp.Domain.Models.Blogs;
using BlogApp.Infrastructure.EFCore.Contexts;

namespace BlogApp.Infrastructure.EFCore.Repositories;

public class BlogRepository(BlogAppDbContext context)
    : EFBaseRepository<Blog, int>(context), IBlogRepository
{
    public async Task<List<Blog>> GetTopReadAsync(int count, CancellationToken cancellationToken = default)
    {
        return await GetAll(false)
            .Where(x => x.PostStatus == PostStatus.Published)
            .OrderByDescending(x => x.ReadCount)
            .Take(count)
            .Include(x => x.Author)
            .Include(x => x.BlogTags)
            .ThenInclude(x => x.Tag)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<Blog>> GetRandomAsync(RandomBlogArgs args, CancellationToken cancellationToken = default)
    {
        var query = GetAll(false)
            .Where(x => x.PostStatus == PostStatus.Published);

        if (args.Categories is { Count: > 0 })
            query = query.Where(x => x.BlogCategories.Any(b => b.Category!.Name.Equals(args.Categories)));

        return await query
            .OrderBy(_ => EF.Functions.Random())
            .Take(args.Count)
            .Include(x => x.Author)
            .Include(x => x.BlogTags)
            .ThenInclude(x => x.Tag)
            .ToListAsync(cancellationToken);
    }
}