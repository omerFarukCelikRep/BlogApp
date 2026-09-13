using BlogApp.Core.Security.Attributes;

namespace BlogApp.Core.Security.Abstractions;

public interface IXssSanitizer
{
    /// <summary>
    /// Strips ALL HTML tags — plain text only.
    /// </summary>
    string StripHtml(string input);

    /// <summary>
    /// Strips dangerous tags/attributes — keeps safe HTML from allowlist.
    /// </summary>
    string SanitizeHtml(string input);

    /// <summary>
    /// Encodes special HTML characters.
    /// </summary>
    string HtmlEncode(string input);

    /// <summary>
    /// Sanitizes based on SanitizeAttribute options.
    /// </summary>
    string Sanitize(string input, SanitizeAttribute attribute);
}