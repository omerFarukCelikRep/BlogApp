namespace BlogApp.Domain.Models.Categories;

public record CategoryResult(
    int Id,
    string Name,
    string Slug,
    int? BlogCount = null
)
{
    public static explicit operator CategoryResult(Category category)
    {
        return new(
            Id: category.Id,
            Name: category.Name,
            Slug: category.Slug,
            BlogCount: category.BlogCategories.Count
        );
    }
}