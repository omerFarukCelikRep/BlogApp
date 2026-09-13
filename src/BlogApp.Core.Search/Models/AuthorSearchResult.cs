namespace BlogApp.Core.Search.Models;

public record AuthorSearchResult(
    Guid    Id,
    string  FullName,
    string  Username,
    string? Bio,
    string? ProfilePicture,
    int     PostCount,
    double  Score);