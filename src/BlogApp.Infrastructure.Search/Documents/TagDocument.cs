namespace BlogApp.Infrastructure.Search.Documents;

public record TagDocument
{
    public string Name      { get; init; } = string.Empty;
    public string Slug      { get; init; } = string.Empty;
    public int    BlogCount { get; init; }
}