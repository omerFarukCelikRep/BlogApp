namespace BlogApp.Domain.Models.Categories;

public record CategoryDetailsResult(
    int Id,
    string Name,
    string Slug,
    int BlogCount,
    CategoryResult? Parent = null,
    List<CategoryDetailsResult>? Children = null);