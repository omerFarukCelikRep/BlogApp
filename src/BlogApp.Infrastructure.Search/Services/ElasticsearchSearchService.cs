using BlogApp.Core.Search.Abstractions;
using BlogApp.Core.Search.Models;
using BlogApp.Core.Search.Options;
using BlogApp.Infrastructure.EFCore.Contexts;
using BlogApp.Infrastructure.Search.Documents;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Elastic.Clients.Elasticsearch;
using Elastic.Clients.Elasticsearch.Core.Search;
using Elastic.Clients.Elasticsearch.QueryDsl;
using Microsoft.EntityFrameworkCore;

namespace BlogApp.Infrastructure.Search.Services;

public partial class ElasticsearchSearchService(
    ElasticsearchClient elasticsearchClient,
    BlogAppDbContext context,
    IOptions<SearchOptions> options,
    ILogger<ElasticsearchSearchService> logger) : ISearchService
{
    private const string Provider = "Elasticsearch";
    private readonly string _indexName = options.Value.Elasticsearch.IndexName;

    private async Task<List<BlogSearchResult>> SearchBlogsAsync(SearchFilter filter,
        CancellationToken cancellationToken = default)
    {
        var must = new List<Query>();

        // Multi-match search across title + content
        if (!string.IsNullOrWhiteSpace(filter.Query))
        {
            must.Add(new MultiMatchQuery
            {
                Fields = new[] { "title^3", "content", "tags" },
                Query = filter.Query,
                Type = TextQueryType.BestFields,
                Fuzziness = new Fuzziness("AUTO"),
            });
        }

        if (!string.IsNullOrEmpty(filter.Tag))
            must.Add(new TermQuery("tags", filter.Tag));

        if (!string.IsNullOrEmpty(filter.Author))
            must.Add(new TermQuery("authorUsername", filter.Author));

        if (!string.IsNullOrEmpty(filter.DateFrom) ||
            !string.IsNullOrEmpty(filter.DateTo))
        {
            _ = DateTime.TryParse(filter.DateFrom, out var dateFrom);
            _ = DateTime.TryParse(filter.DateTo, out var dateTo);
            must.Add(new DateRangeQuery("publishDate")
            {
                Gte = dateFrom,
                Lte = dateTo,
            });
        }

        var response = await elasticsearchClient.SearchAsync<BlogDocument>(s => s
                .Indices($"{_indexName}-blogs")
                .From((filter.Page - 1) * filter.PageSize)
                .Size(filter.PageSize)
                .Query(q => q.Bool(b => b.Must([.. must])))
                .Highlight(h => h.Fields(kvp =>
                    {
                        kvp.Add("title", new HighlightField() { NumberOfFragments = 0 });
                        kvp.Add("content", new HighlightField() { NumberOfFragments = 2, FragmentSize = 150 });
                    })
                    .PreTags("<mark>")
                    .PostTags("</mark>"))
                .Sort(filter.SortBy switch
                {
                    "most-read" => sort => sort.Field("readCount", SortOrder.Desc),
                    "latest" => sort => sort.Field("publishedAt", SortOrder.Desc),
                    _ => sort => sort.Score(a => a.Order(SortOrder.Desc)),
                }),
            cancellationToken);

        if (!response.IsValidResponse)
        {
            LogElasticsearchBlogSearchFailedError(response.DebugInformation);
            return [];
        }

        return
        [
            .. response.Hits.Select(hit => new BlogSearchResult(
                Id: int.Parse(hit.Id),
                Title: hit.Source!.Title,
                Excerpt: hit.Source.Excerpt,
                Slug: hit.Source.Slug,
                Thumbnail: hit.Source.Thumbnail,
                AuthorFullName: hit.Source.AuthorFullName,
                AuthorUsername: hit.Source.AuthorUsername,
                ReadingTimeInMinutes: hit.Source.ReadingTimeInMinutes,
                ReadCount: hit.Source.ReadCount,
                PublishedAt: hit.Source.PublishDate,
                Tags: hit.Source.Tags,
                Score: hit.Score ?? 0,
                HighlightedTitle: hit.Highlight?.TryGetValue("title", out var title) is true
                    ? title.FirstOrDefault()
                    : null,
                HighlightedExcerpt: hit.Highlight?.TryGetValue("content", out var content) is true
                    ? content.FirstOrDefault()
                    : null))
        ];
    }

    private async Task<List<TagSearchResult>> SearchTagsAsync(SearchFilter filter,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(filter.Query)) return [];

        var response = await elasticsearchClient.SearchAsync<TagDocument>(s => s
                .Indices($"{_indexName}-tags")
                .Size(10)
                .Query(q => q.MultiMatch(m => m
                    .Fields(new[] { "name^2", "slug" })
                    .Query(filter.Query)
                    .Fuzziness(new Fuzziness("AUTO")))),
            cancellationToken);

        if (!response.IsValidResponse) return [];

        return
        [
            .. response.Hits.Select(hit => new TagSearchResult(
                Id: int.Parse(hit.Id),
                Name: hit.Source!.Name,
                Slug: hit.Source.Slug,
                PostCount: hit.Source.BlogCount,
                Score: hit.Score ?? 0))
        ];
    }

    private async Task<List<AuthorSearchResult>> SearchAuthorsAsync(SearchFilter filter,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(filter.Query)) return [];

        var response = await elasticsearchClient.SearchAsync<AuthorDocument>(s => s
                .Indices($"{_indexName}-authors")
                .Size(10)
                .Query(q => q.MultiMatch(m => m
                    .Fields(new[] { "fullName^3", "username^2", "bio" })
                    .Query(filter.Query)
                    .Fuzziness(new Fuzziness("AUTO")))),
            cancellationToken);

        if (!response.IsValidResponse) return [];

        return
        [
            .. response.Hits.Select(hit => new AuthorSearchResult(
                Id: Guid.Parse(hit.Id),
                FullName: hit.Source!.FullName,
                Username: hit.Source.Username,
                Bio: hit.Source.Bio,
                ProfilePicture: hit.Source.ProfilePicture,
                PostCount: hit.Source.BlogCount,
                Score: hit.Score ?? 0))
        ];
    }

    public async Task<SearchResult> SearchAsync(SearchFilter filter, CancellationToken cancellationToken = default)
    {
        LogElasticsearchSearchQueryTypeType(filter.Query, filter.Type);

        var blogs = new List<BlogSearchResult>();
        var tags = new List<TagSearchResult>();
        var authors = new List<AuthorSearchResult>();

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
            TotalPages: (int)Math.Ceiling(
                (double)blogs.Count / filter.PageSize),
            Page: filter.Page,
            PageSize: filter.PageSize,
            Query: filter.Query,
            Provider: Provider,
            Blogs: blogs,
            Tags: tags,
            Authors: authors);
    }

    public async Task IndexBlogAsync(int blogId, CancellationToken cancellationToken = default)
    {
        var blog = await context.Blogs
            .AsNoTracking()
            .Where(b => b.Id == blogId)
            .Include(b => b.Author)
            .Include(b => b.BlogTags)
            .ThenInclude(bt => bt.Tag)
            .FirstOrDefaultAsync(cancellationToken);

        if (blog is null)
            return;

        var doc = new BlogDocument()
        {
            Title = blog.Title,
            Content = blog.Content,
            Excerpt = blog.Content.Length > 300
                ? blog.Content[..300]
                : string.Empty,
            Slug = blog.Slug,
            Thumbnail = blog.Thumbnail,
            AuthorFullName = blog.Author!.FullName,
            AuthorUsername = blog.Author.Username,
            ReadingTimeInMinutes = blog.ReadingTimeInMinutes,
            ReadCount = blog.ReadCount,
            PublishDate = blog.PublishDate!.Value,
            Tags = [.. blog.BlogTags.Select(bt => bt.Tag!.Name)],
            IsPublished = blog.IsPublished()
        };

        await elasticsearchClient.IndexAsync(
            doc,
            i => i
                .Index($"{_indexName}-blogs")
                .Id(blogId.ToString()),
            cancellationToken);

        LogIndexedBlogBlogId(blogId);
    }

    public async Task RemoveBlogFromIndexAsync(int blogId, CancellationToken cancellationToken = default)
    {
        await elasticsearchClient.DeleteAsync($"{_indexName}-blogs", blogId.ToString(), cancellationToken);

        LogRemovedBlogBlogIdFromİndex(blogId);
    }

    [LoggerMessage(LogLevel.Debug, "Elasticsearch search: {Query} type={Type}")]
    partial void LogElasticsearchSearchQueryTypeType(string query, string type);

    [LoggerMessage(LogLevel.Warning, "Elasticsearch blog search failed: {Error}")]
    partial void LogElasticsearchBlogSearchFailedError(string error);

    [LoggerMessage(LogLevel.Debug, "Indexed blog {BlogId}")]
    partial void LogIndexedBlogBlogId(int blogId);

    [LoggerMessage(LogLevel.Debug, "Removed blog {BlogId} from index")]
    partial void LogRemovedBlogBlogIdFromİndex(int blogId);
}