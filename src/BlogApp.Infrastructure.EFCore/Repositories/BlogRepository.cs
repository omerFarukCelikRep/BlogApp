using System.Globalization;
using BlogApp.Core.DataAccess.Models;
using BlogApp.Core.EFCore.Extensions;
using BlogApp.Domain.Abstractions.Repositories;
using BlogApp.Domain.Enums;
using BlogApp.Domain.Models.Admins;
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
            query = query.Where(x => x.BlogCategories.Any(b => args.Categories.Any(c => c == b.CategoryId)));

        return await query
            .OrderBy(_ => EF.Functions.Random())
            .Take(args.Count)
            .Include(x => x.Author)
            .Include(x => x.BlogTags)
            .ThenInclude(x => x.Tag)
            .ToListAsync(cancellationToken);
    }

    public async Task<Blog?> GetDetailsAsync(int id, CancellationToken cancellationToken = default)
    {
        return await GetAll(false)
            .Where(x => x.Id == id)
            .Include(x => x.Author)
            .Include(x => x.Likes)
            .Include(x => x.BlogTags)
            .ThenInclude(x => x.Tag)
            .Include(x => x.BlogCategories)
            .ThenInclude(x => x.Category)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<IPaginate<Blog>> GetAllByTagAsync(GetBlogByTagArgs args,
        CancellationToken cancellationToken = default)
    {
        var query = GetAll(false)
            .Where(x => x.PostStatus == PostStatus.Published && x.BlogTags.Any(t => t.Id == args.TagId))
            .Include(x => x.Author)
            .Include(x => x.Likes)
            .Include(x => x.BlogTags)
            .ThenInclude(x => x.Tag);

        var ordered = args.SortBy?.ToLowerInvariant() switch
        {
            "most-read" => query.OrderByDescending(x => x.ReadCount),
            _ => query.OrderByDescending(x => x.CreatedDate)
        };

        return await ordered.ToPaginateAsync(args.Page!.Value, args.PageSize!.Value, cancellationToken);
    }

    public async Task<IPaginate<Blog>> GetAllByCategoryAsync(GetBlogByCategoryArgs args,
        CancellationToken cancellationToken = default)
    {
        var query = GetAll(false)
            .Where(b => b.IsPublished() && b.BlogCategories.Any(bc => args.Categories!.Contains(bc.CategoryId)))
            .Include(b => b.Author)
            .Include(b => b.Likes)
            .Include(b => b.BlogTags)
            .ThenInclude(bt => bt.Tag)
            .Include(b => b.BlogCategories)
            .ThenInclude(bc => bc.Category);

        var ordered = args.SortBy?.ToLowerInvariant() switch
        {
            "most-read" => query.OrderByDescending(x => x.ReadCount),
            _ => query.OrderByDescending(b => b.CreatedDate)
        };

        return await query.ToPaginateAsync(args.Page!.Value, args.PageSize!.Value, cancellationToken);
    }

    public async Task<IPaginate<Blog>> GetAllByAuthorAsync(GetBlogByAuthorArgs args,
        CancellationToken cancellationToken = default)
    {
        var query = GetAll(false)
            .Where(b => b.AuthorId == args.AuthorId && b.IsPublished())
            .Include(b => b.Likes)
            .Include(b => b.Comments)
            .Include(b => b.BlogTags)
            .ThenInclude(bt => bt.Tag);

        var ordered = args.SortBy?.ToLowerInvariant() switch
        {
            "most-read" => query.OrderByDescending(b => b.ReadCount),
            _ => query.OrderByDescending(b => b.CreatedDate),
        };

        return await ordered.ToPaginateAsync(args.Page!.Value, args.PageSize!.Value, cancellationToken);
    }

    public async Task<List<Blog>> GetRecentAsync(int count, CancellationToken cancellationToken = default)
    {
        return await GetAll(false)
            .Where(x => x.PostStatus == PostStatus.Published)
            .OrderByDescending(x => x.CreatedDate)
            .Take(count)
            .Include(x => x.Author)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<DailyCountResult>> GetDailyCountAsync(DateTime from,
        CancellationToken cancellationToken = default)
    {
        return await GetAll(false)
            .Where(x => x.CreatedDate >= from)
            .OrderBy(x => x.CreatedDate)
            .GroupBy(x => x.CreatedDate.Date)
            .Select(x => new DailyCountResult(x.Key.ToString(CultureInfo.CurrentCulture), x.Count()))
            .ToListAsync(cancellationToken);
    }

    public Task<IPaginate<Blog>> GetAllAsync(GetAllForAdminsArgs args, CancellationToken cancellationToken = default)
    {
        var query = GetAll(false)
            .Include(b => b.Author)
            .Include(b => b.Likes)
            .Include(b => b.Comments)
            .Include(b => b.BlogCategories)
            .ThenInclude(bt => bt.Category)
            .Include(b => b.BlogTags)
            .ThenInclude(bt => bt.Tag)
            .AsQueryable();
        if (!string.IsNullOrEmpty(args.Search))
            query = query.Where(b => EF.Functions.ILike(b.Title, $"%{args.Search}%")
                                     || EF.Functions.ILike(b.Content, $"%{args.Search}%"));

        if (args.Status.HasValue)
            query = query.Where(b => b.PostStatus == args.Status);

        if (!string.IsNullOrEmpty(args.AuthorName))
            query = query.Where(b => EF.Functions.ILike(b.Author!.FullName, $"%{args.AuthorName}%"));

        return query
            .OrderByDescending(b => b.CreatedDate)
            .ToPaginateAsync(args.Page!.Value, args.PageSize!.Value, cancellationToken);
    }
}