namespace BlogApp.Infrastructure.Search.Documents;

public record BlogDocument
{
    public string Title { get; init; } = string.Empty;
    public string Content { get; init; } = string.Empty;
    public string Excerpt { get; init; } = string.Empty;
    public string Slug { get; init; } = string.Empty;
    public string? Thumbnail { get; init; }
    public string AuthorFullName { get; init; } = string.Empty;
    public string AuthorUsername { get; init; } = string.Empty;
    public int ReadingTimeInMinutes { get; init; }
    public int ReadCount { get; init; }
    public DateTime PublishDate { get; init; }
    public List<string> Tags { get; init; } = [];
    public bool IsPublished { get; init; }
}