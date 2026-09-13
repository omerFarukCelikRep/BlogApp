using BlogApp.Core.Search.Abstractions;
using BlogApp.Core.Search.Models;
using BlogApp.Domain.Enums;
using BlogApp.Infrastructure.EFCore.Contexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BlogApp.Infrastructure.Search.Services;

public partial class PostgresSearchService(BlogAppDbContext context, ILogger<PostgresSearchService> logger)
    : ISearchService
{
    private const string Provider = "PostgreSQL";

    private async Task<List<BlogSearchResult>> SearchBlogsAsync(SearchFilter filter,
        CancellationToken cancellationToken = default)
    {
        var query = context.Blogs
            .Where(b => b.PostStatus == PostStatus.Published)
            .AsNoTracking();

        if (!string.IsNullOrWhiteSpace(filter.Query))
        {
            var q = filter.Query.ToLower();
            query = query.Where(b => EF.Functions.ILike(b.Title, $"%{q}%")
                                     || EF.Functions.ILike(b.Content, $"%{q}%")
                                     || EF.Functions.ILike(b.Slug, $"%{q}%"));
        }

        if (!string.IsNullOrEmpty(filter.Tag))
            query = query.Where(b => b.BlogTags.Any(bt => EF.Functions.ILike(bt.Tag!.Slug, filter.Tag)));

        if (!string.IsNullOrEmpty(filter.Author))
            query = query.Where(b => EF.Functions.ILike(b.Author!.Username, filter.Author));

        if (DateTime.TryParse(filter.DateFrom, out var dateFrom))
            query = query.Where(b => b.CreatedDate >= dateFrom);

        if (DateTime.TryParse(filter.DateTo, out var dateTo))
            query = query.Where(b => b.CreatedDate <= dateTo);

        var ordered = filter.SortBy switch
        {
            "most-read" => query.OrderByDescending(b => b.ReadCount),
            "latest" => query.OrderByDescending(b => b.CreatedDate),
            _ => query.OrderByDescending(b => EF.Functions.ILike(b.Title, $"%{filter.Query}%") ? 2 : 1)
        };

        var result = await ordered
            .Skip((filter.Page - 1) * filter.PageSize)
            .Take(filter.PageSize)
            .Include(b => b.Author)
            .Include(b => b.BlogTags)
            .ThenInclude(b => b.Tag)
            .Select(b => new BlogSearchResult(
                Id: b.Id,
                Title: b.Title,
                Excerpt: b.Content.Length > 160
                    ? b.Content[..160] + "..."
                    : b.Content,
                Slug: b.Slug,
                Thumbnail: b.Thumbnail,
                AuthorFullName: b.Author!.FullName,
                AuthorUsername: b.Author.Username,
                ReadingTimeInMinutes: b.ReadingTimeInMinutes,
                ReadCount: b.ReadCount,
                PublishedAt: b.CreatedDate.DateTime,
                Tags: b.BlogTags
                    .Select(bt => bt.Tag!.Name)
                    .ToList(),
                Score: EF.Functions.ILike(b.Title, $"%{filter.Query}%")
                    ? 2.0
                    : 1.0))
            .ToListAsync(cancellationToken);

        return result;
    }

    private async Task<List<TagSearchResult>> SearchTagsAsync(
        SearchFilter filter,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(filter.Query))
            return [];

        var q = filter.Query.ToLower();

        return await context.Tags
            .AsNoTracking()
            .Where(t => EF.Functions.ILike(t.Name, $"%{q}%")
                        || EF.Functions.ILike(t.Slug, $"%{q}%"))
            .OrderByDescending(t => t.BlogTags.Count)
            .Take(10)
            .Select(t => new TagSearchResult(
                Id: t.Id,
                Name: t.Name,
                Slug: t.Slug,
                PostCount: t.BlogTags.Count(bt => bt.Blog!.IsPublished()),
                Score: EF.Functions.ILike(t.Name, filter.Query) ? 2.0 : 1.0))
            .ToListAsync(cancellationToken);
    }

    private async Task<List<AuthorSearchResult>> SearchAuthorsAsync(
        SearchFilter filter,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(filter.Query))
            return [];

        var q = filter.Query.ToLower();

        return await context.Users
            .AsNoTracking()
            .Where(u => EF.Functions.ILike(u.FirstName + " " + u.LastName, $"%{q}%")
                        || EF.Functions.ILike(u.Username, $"%{q}%")
                        || (u.Bio != null && EF.Functions.ILike(u.Bio, $"%{q}%")))
            .OrderByDescending(u => u.Blogs.Count(b => b.PostStatus == PostStatus.Published))
            .Take(10)
            .Select(u => new AuthorSearchResult(
                Id: u.Id,
                FullName: u.FirstName + " " + u.LastName,
                Username: u.Username,
                Bio: u.Bio,
                ProfilePicture: u.ProfilePicture,
                PostCount: u.Blogs.Count(b => b.IsPublished()),
                Score: EF.Functions.ILike(u.Username, filter.Query)
                    ? 2.0
                    : 1.0))
            .ToListAsync(cancellationToken);
    }

    public async Task<SearchResult> SearchAsync(SearchFilter filter, CancellationToken cancellationToken = default)
    {
        LogPostgresqlSearchQueryTypeType(filter.Query, filter.Type);

        List<BlogSearchResult> blogs = [];
        List<TagSearchResult> tags = [];
        List<AuthorSearchResult> authors = [];

        var searchType = filter.Type.ToLowerInvariant();
        if (searchType is "all" or "blogs")
            blogs = await SearchBlogsAsync(filter, cancellationToken);

        if (searchType is "all" or "tags")
            tags = await SearchTagsAsync(filter, cancellationToken);

        if (searchType is "all" or "authors")
            authors = await SearchAuthorsAsync(filter, cancellationToken);

        var totalCount = blogs.Count + tags.Count + authors.Count;
        return new SearchResult(
            TotalCount: totalCount,
            TotalPages: (int)Math.Ceiling((double)blogs.Count / filter.PageSize),
            Page: filter.Page,
            PageSize: filter.PageSize,
            Query: filter.Query,
            Provider: Provider,
            Blogs: blogs,
            Tags: tags,
            Authors: authors);
    }

    public Task IndexBlogAsync(int blogId, CancellationToken cancellationToken = default) => Task.CompletedTask;

    public Task RemoveBlogFromIndexAsync(int blogId, CancellationToken cancellationToken = default) =>
        Task.CompletedTask;

    [LoggerMessage(LogLevel.Debug, "PostgreSQL search: {Query} type={Type}")]
    partial void LogPostgresqlSearchQueryTypeType(string query, string type);
}