using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using BlogApp.Core.Security.Abstractions;
using BlogApp.Core.Security.Attributes;
using BlogApp.Core.Security.Options;
using Microsoft.Extensions.Options;

namespace BlogApp.Core.Security.Sanitizers;

public partial class XssSanitizer(IOptions<XssOptions> options) : IXssSanitizer
{
    private readonly XssOptions _options = options.Value;

    // Matches any HTML tag
    [GeneratedRegex("<[^>]*>", RegexOptions.Compiled)]
    private static partial Regex HtmlTagRegex();

    // Matches script tags and content
    [GeneratedRegex("""<script\b[^<]*(?:(?!<\/script>)<[^<]*)*<\/script>""",
        RegexOptions.IgnoreCase | RegexOptions.Compiled)]
    private static partial Regex ScriptTagRegex();

    // Matches style tags and content
    [GeneratedRegex("""<style\b[^<]*(?:(?!<\/style>)<[^<]*)*<\/style>""",
        RegexOptions.IgnoreCase | RegexOptions.Compiled)]
    private static partial Regex StyleTagRegex();

    // Matches dangerous event attributes (onclick, onerror, onload etc.)
    [GeneratedRegex("""\bon\w+\s*=\s*(?:["'][^"']*["']|\S+)""", RegexOptions.IgnoreCase | RegexOptions.Compiled)]
    private static partial Regex EventAttributeRegex();

    // Matches JavaScript: in href/src
    [GeneratedRegex("""(?:href|src|action)\s*=\s*["']?\s*javascript:""",
        RegexOptions.IgnoreCase | RegexOptions.Compiled)]
    private static partial Regex JavascriptUrlRegex();

    // Matches data: URLs (can carry scripts)
    [GeneratedRegex("""(?:href|src)\s*=\s*["']?\s*data:""", RegexOptions.IgnoreCase | RegexOptions.Compiled)]
    private static partial Regex DataUrlRegex();

    // Matches vbscript: URLs
    [GeneratedRegex("""(?:href|src)\s*=\s*["']?\s*vbscript:""", RegexOptions.IgnoreCase | RegexOptions.Compiled)]
    private static partial Regex VbscriptUrlRegex();

    // Matches HTML comments (can hide scripts)
    [GeneratedRegex("<!--.*?-->", RegexOptions.Singleline | RegexOptions.Compiled)]
    private static partial Regex HtmlCommentRegex();

    // Matches HTML attributes
    [GeneratedRegex("""(?<name>\w[\w-]*)(?:\s*=\s*(?:"(?<value>[^"]*)"|'(?<value>[^']*)'|(?<value>[^\s>]*)))?""",
        RegexOptions.Compiled)]
    private static partial Regex AttributeRegex();

    // Matches dangerous tags by name
    private static readonly HashSet<string> _dangerousTags =
    [
        "script", "style", "iframe", "object", "embed",
        "applet", "base", "form", "input", "button",
        "link", "meta", "frame", "frameset",
    ];

    private string ProcessTags(string input)
    {
        var result = new StringBuilder();
        var i = 0;

        while (i < input.Length)
        {
            if (input[i] != '<')
            {
                result.Append(input[i]);
                i++;
                continue;
            }

            // Find end of tag
            var tagEnd = input.IndexOf('>', i);
            if (tagEnd == -1)
            {
                // Unclosed tag — encode the < and move on
                result.Append("&lt;");
                i++;
                continue;
            }

            var tag = input[i..(tagEnd + 1)];
            var tagName = ExtractTagName(tag);

            if (string.IsNullOrEmpty(tagName))
            {
                i = tagEnd + 1;
                continue;
            }

            // Closing tag
            var isClosing = tagName.StartsWith('/');
            var cleanName = isClosing ? tagName[1..] : tagName;

            if (_dangerousTags.Contains(cleanName.ToLowerInvariant()))
            {
                // Skip dangerous tag entirely
                i = tagEnd + 1;
                continue;
            }

            if (_options.AllowedTags.Contains(cleanName.ToLowerInvariant()))
            {
                // Keep allowed tag but sanitize its attributes
                if (isClosing)
                    result.Append($"</{cleanName}>");
                else
                    result.Append(BuildSafeTag(cleanName, tag));
            }
            // else — unknown tag, skip silently

            i = tagEnd + 1;
        }

        return result.ToString();
    }

    private static string ExtractTagName(string tag)
    {
        // Remove < and >
        var inner = tag.TrimStart('<').TrimEnd('>').Trim();
        if (string.IsNullOrEmpty(inner)) return string.Empty;

        // Get first word (tag name)
        var spaceIndex = inner.IndexOf(' ');
        return spaceIndex == -1
            ? inner.ToLowerInvariant()
            : inner[..spaceIndex].ToLowerInvariant();
    }

    private string BuildSafeTag(string tagName, string originalTag)
    {
        var sb = new StringBuilder($"<{tagName}");

        // Extract and sanitize attributes
        var attrMatches = AttributeRegex().Matches(originalTag);

        foreach (Match match in attrMatches)
        {
            var attrName = match.Groups["name"].Value.ToLowerInvariant();
            var attrValue = match.Groups["value"].Value;

            if (!_options.AllowedAttributes.Contains(attrName))
                continue;

            switch (attrName)
            {
                // Validate URL attributes
                case "href" or "src" or "action" when !IsAllowedUrl(attrValue):
                    continue;
                // Force safe rel on links
                case "target" when attrValue == "_blank":
                    sb.Append(" target=\"_blank\" rel=\"noopener noreferrer\"");
                    continue;
                default:
                    sb.Append($" {attrName}=\"{HtmlEncode(attrValue)}\"");
                    break;
            }
        }

        sb.Append('>');
        return sb.ToString();
    }

    private bool IsAllowedUrl(string url)
    {
        if (string.IsNullOrWhiteSpace(url))
            return false;

        // Relative URLs are fine
        if (!url.Contains("://") && !url.StartsWith("//"))
            return true;

        // Check scheme
        return _options.AllowedSchemes.Any(scheme =>
            url.StartsWith($"{scheme}://", StringComparison.OrdinalIgnoreCase));
    }

    public string StripHtml(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return input;

        var result = input;

        result = ScriptTagRegex().Replace(result, string.Empty);
        result = StyleTagRegex().Replace(result, string.Empty);

        result = HtmlCommentRegex().Replace(result, string.Empty);

        result = HtmlTagRegex().Replace(result, string.Empty);

        result = WebUtility.HtmlDecode(result);

        return result.Trim();
    }

    public string SanitizeHtml(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return input;

        var result = input;

        result = HtmlCommentRegex().Replace(result, string.Empty);

        result = ScriptTagRegex().Replace(result, string.Empty);
        result = StyleTagRegex().Replace(result, string.Empty);

        result = EventAttributeRegex().Replace(result, string.Empty);

        result = JavascriptUrlRegex().Replace(result, "href=\"#\"");
        result = VbscriptUrlRegex().Replace(result, "href=\"#\"");
        result = DataUrlRegex().Replace(result, "src=\"\"");

        result = ProcessTags(result);

        return result.Trim();
    }

    public string HtmlEncode(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return input;

        return input
            .Replace("&", "&amp;")
            .Replace("<", "&lt;")
            .Replace(">", "&gt;")
            .Replace("\"", "&quot;")
            .Replace("'", "&#x27;");
    }

    public string Sanitize(string input, SanitizeAttribute attribute)
    {
        if (string.IsNullOrWhiteSpace(input))
            return input;

        string result;
        result = attribute.AllowHtml
            ? SanitizeHtml(input)
            : StripHtml(input);

        if (attribute.HtmlEncode)
            result = HtmlEncode(result);

        if (attribute.MaxLength > 0 && result.Length > attribute.MaxLength)
            result = result[..attribute.MaxLength];

        return result;
    }
}