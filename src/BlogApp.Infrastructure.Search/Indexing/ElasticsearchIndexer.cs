using BlogApp.Core.Search.Options;
using BlogApp.Domain.Enums;
using BlogApp.Infrastructure.EFCore.Contexts;
using BlogApp.Infrastructure.Search.Documents;
using Elastic.Clients.Elasticsearch;
using Elastic.Clients.Elasticsearch.Core.Bulk;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace BlogApp.Infrastructure.Search.Indexing;

public partial class ElasticsearchIndexer(
    ElasticsearchClient elasticsearchClient,
    BlogAppDbContext context,
    IOptions<SearchOptions> options,
    ILogger<ElasticsearchIndexer> logger)
{
    private const string Analyzer = "standard";
    private readonly string _indexName = options.Value.Elasticsearch.IndexName;

    private async Task CreateBlogIndexAsync(CancellationToken cancellationToken = default)
    {
        var indexName = $"{_indexName}-blogs";
        var exists = await elasticsearchClient.Indices.ExistsAsync(indexName, cancellationToken);
        if (exists.Exists)
            return;

        await elasticsearchClient.Indices.CreateAsync(indexName, i => i
            .Mappings(m => m
                .Properties<BlogDocument>(p => p
                    .Text(t => t.Title, f => f.Analyzer(Analyzer))
                    .Text(t => t.Content, f => f.Analyzer(Analyzer))
                    .Keyword(k => k.Slug)
                    .Keyword(k => k.AuthorUsername)
                    .Text(t => t.AuthorFullName)
                    .Keyword(k => k.Tags)
                    .IntegerNumber(n => n.ReadCount)
                    .Date(d => d.PublishDate)
                    .Boolean(b => b.IsPublished))), cancellationToken);

        LogCreatedIndex(indexName);
    }

    private async Task CreateTagIndexAsync(CancellationToken cancellationToken = default)
    {
        var indexName = $"{_indexName}-tags";
        var exists = await elasticsearchClient.Indices.ExistsAsync(indexName, cancellationToken);
        if (exists.Exists)
            return;

        await elasticsearchClient.Indices.CreateAsync(indexName, i => i
            .Mappings(m => m
                .Properties<TagDocument>(p => p
                    .Text(t => t.Name, f => f.Analyzer(Analyzer))
                    .Keyword(k => k.Slug)
                    .IntegerNumber(n => n.BlogCount))), cancellationToken);
    }

    private async Task CreateAuthorIndexAsync(CancellationToken cancellationToken = default)
    {
        var indexName = $"{_indexName}-authors";
        var exists = await elasticsearchClient.Indices.ExistsAsync(indexName, cancellationToken);
        if (exists.Exists)
            return;

        await elasticsearchClient.Indices.CreateAsync(indexName, i => i
            .Mappings(m => m
                .Properties<AuthorDocument>(p => p
                    .Text(t => t.FullName, f => f.Analyzer(Analyzer))
                    .Keyword(k => k.Username)
                    .Text(t => t.Bio)
                    .IntegerNumber(n => n.BlogCount))), cancellationToken);
    }

    private async Task ReindexBlogsAsync(CancellationToken cancellationToken = default)
    {
        var indexName = $"{_indexName}-blogs";
        var blogs = await context.Blogs
            .AsNoTracking()
            .Where(b => b.PostStatus == PostStatus.Published)
            .Include(b => b.Author)
            .Include(b => b.BlogTags)
            .ThenInclude(bt => bt.Tag)
            .ToListAsync(cancellationToken);

        var ops = blogs
            .Select(b => new BulkIndexOperation<BlogDocument>(new BlogDocument()
            {
                Title = b.Title,
                Content = b.Content,
                Excerpt = b.Content.Length > 300
                    ? b.Content[..300]
                    : b.Content,
                Slug = b.Slug,
                Thumbnail = b.Thumbnail,
                AuthorFullName = b.Author!.FullName,
                AuthorUsername = b.Author.Username,
                ReadingTimeInMinutes = b.ReadingTimeInMinutes,
                ReadCount = b.ReadCount,
                PublishDate = b.PublishDate!.Value,
                Tags = [.. b.BlogTags.Select(bt => bt.Tag!.Name)],
                IsPublished = true
            }) { Id = b.Id.ToString() });
        if (!ops.Any())
            return;

        await elasticsearchClient.BulkAsync(r => r
            .Index(indexName)
            .IndexMany(blogs.Select(b => new BlogDocument()
            {
                Title = b.Title,
                Content = b.Content,
                Excerpt = b.Content.Length > 300
                    ? b.Content[..300]
                    : b.Content,
                Slug = b.Slug,
                Thumbnail = b.Thumbnail,
                AuthorFullName = b.Author!.FullName,
                AuthorUsername = b.Author.Username,
                ReadingTimeInMinutes = b.ReadingTimeInMinutes,
                ReadCount = b.ReadCount,
                PublishDate = b.PublishDate!.Value,
                Tags = [.. b.BlogTags.Select(bt => bt.Tag!.Name)],
                IsPublished = true
            })), cancellationToken);

        LogIndexedCountBlogs(blogs.Count);
    }

    private async Task ReindexTagsAsync(CancellationToken cancellationToken = default)
    {
        var indexName = $"{_indexName}-tags";
        var tags = await context.Tags
            .AsNoTracking()
            .Include(t => t.BlogTags)
            .ToListAsync(cancellationToken);

        var docs = tags.Select(t => new TagDocument
        {
            Name = t.Name,
            Slug = t.Slug,
            BlogCount = t.BlogTags.Count,
        }).ToList();

        await elasticsearchClient.BulkAsync(r => r
            .Index(indexName)
            .IndexMany(docs), cancellationToken);

        LogIndexedCountTags(tags.Count);
    }

    private async Task ReindexAuthorsAsync(CancellationToken cancellationToken = default)
    {
        var indexName = $"{_indexName}-authors";
        var authors = await context.Users
            .AsNoTracking()
            .Include(u => u.Blogs)
            .ToListAsync(cancellationToken);

        var docs = authors.Select(u => new AuthorDocument
        {
            FullName = u.FullName,
            Username = u.Username,
            Bio = u.Bio,
            ProfilePicture = u.ProfilePicture,
            BlogCount = u.Blogs.Count(b => b.IsPublished()),
        }).ToList();

        await elasticsearchClient.BulkAsync(r => r
            .Index(indexName)
            .IndexMany(docs), cancellationToken);

        LogIndexedCountAuthors(authors.Count);
    }

    public async Task CreateIndicesAsync(CancellationToken cancellationToken = default)
    {
        await CreateBlogIndexAsync(cancellationToken);
        await CreateTagIndexAsync(cancellationToken);
        await CreateAuthorIndexAsync(cancellationToken);
    }

    public async Task ReIndexAllAsync(CancellationToken cancellationToken = default)
    {
        LogStartingFullReIndex();

        await ReindexBlogsAsync(cancellationToken);
        await ReindexTagsAsync(cancellationToken);
        await ReindexAuthorsAsync(cancellationToken);

        LogFullReIndexComplete();
    }

    [LoggerMessage(LogLevel.Information, "Starting full reindex...")]
    partial void LogStartingFullReIndex();

    [LoggerMessage(LogLevel.Information, "Full reindex complete")]
    partial void LogFullReIndexComplete();

    [LoggerMessage(LogLevel.Information, "Created index {indexName}")]
    partial void LogCreatedIndex(string indexName);

    [LoggerMessage(LogLevel.Information, "Indexed {Count} blogs")]
    partial void LogIndexedCountBlogs(int count);

    [LoggerMessage(LogLevel.Information, "Indexed {Count} tags")]
    partial void LogIndexedCountTags(int count);

    [LoggerMessage(LogLevel.Information, "Indexed {Count} authors")]
    partial void LogIndexedCountAuthors(int count);
}