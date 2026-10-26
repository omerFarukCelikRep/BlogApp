namespace BlogApp.Domain.Models.Tags;

public record TagResult(
    int Id,
    string Name,
    string Slug,
    int BlogCount)
{
    public static explicit operator TagResult(Tag tag)
    {
        return new(
            Id: tag.Id,
            Name: tag.Name,
            Slug: tag.Slug,
            BlogCount: tag.BlogTags.Count
        );
    }
}