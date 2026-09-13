namespace BlogApp.Core.Search.Models;

public record SearchResult(
    int                   TotalCount,
    int                   TotalPages,
    int                   Page,
    int                   PageSize,
    string                Query,
    string                Provider,
    List<BlogSearchResult>   Blogs,
    List<TagSearchResult>    Tags,
    List<AuthorSearchResult> Authors);