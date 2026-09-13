namespace BlogApp.Core.Search.Models;

public record BlogSearchResult(
    int      Id,
    string   Title,
    string   Excerpt,
    string   Slug,
    string?  Thumbnail,
    string   AuthorFullName,
    string   AuthorUsername,
    int      ReadingTimeInMinutes,
    int      ReadCount,
    DateTime PublishedAt,
    List<string> Tags,
    double   Score,
    string?  HighlightedTitle   = null,
    string?  HighlightedExcerpt = null);