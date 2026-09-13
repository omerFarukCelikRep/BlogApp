namespace BlogApp.Infrastructure.Search.Documents;

public record AuthorDocument
{
    public string  FullName       { get; init; } = string.Empty;
    public string  Username       { get; init; } = string.Empty;
    public string? Bio            { get; init; }
    public string? ProfilePicture { get; init; }
    public int     BlogCount      { get; init; }
}