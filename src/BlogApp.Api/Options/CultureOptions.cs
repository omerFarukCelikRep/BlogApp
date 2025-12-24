namespace BlogApp.Api.Options;

public class CultureOptions
{
    public const string SectionName = "Culture";

    public string Default { get; set; } = null!;
    public List<string> Supported { get; set; } = null!;
}