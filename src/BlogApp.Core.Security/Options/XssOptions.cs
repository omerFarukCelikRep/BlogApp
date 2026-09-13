namespace BlogApp.Core.Security.Options;

public class XssOptions
{
    public const string Section = "Xss";

    /// <summary>
    /// Tags allowed when AllowHtml = true on [Sanitize] attribute.
    /// </summary>
    public HashSet<string> AllowedTags { get; set; } =
    [
        "p", "br", "strong", "b", "em", "i", "u", "s",
        "h1", "h2", "h3", "h4", "h5", "h6",
        "ul", "ol", "li",
        "a", "img",
        "code", "pre", "blockquote",
        "table", "thead", "tbody", "tr", "th", "td",
        "div", "span", "hr",
    ];

    /// <summary>
    /// Attributes allowed on permitted tags.
    /// </summary>
    public HashSet<string> AllowedAttributes { get; set; } =
    [
        "href", "src", "alt", "title",
        "class", "id",
        "target", "rel",
        "width", "height",
        "colspan", "rowspan",
    ];

    /// <summary>
    /// URL schemes allowed in href/src attributes.
    /// </summary>
    public HashSet<string> AllowedSchemes { get; set; } =
    [
        "http", "https", "mailto",
    ];

    /// <summary>
    /// When true — strip ALL html by default.
    /// Properties marked [Sanitize(AllowHtml = true)] still allow safe HTML.
    /// </summary>
    public bool StripHtmlByDefault { get; set; } = true;
}