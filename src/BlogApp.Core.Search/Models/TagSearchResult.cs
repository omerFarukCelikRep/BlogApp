namespace BlogApp.Core.Search.Models;

public record TagSearchResult(
    int    Id,
    string Name,
    string Slug,
    int    PostCount,
    double Score);