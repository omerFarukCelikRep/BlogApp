using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace BlogApp.Domain.Utils;

public static partial class BlogHelper
{
    private static void ExtractText(JsonElement root, StringBuilder sb)
    {
        if (root.ValueKind != JsonValueKind.Object)
            return;

        if (root.TryGetProperty("text", out var textProp) && textProp.ValueKind == JsonValueKind.String)
        {
            var text = textProp.GetString();
            if (!string.IsNullOrEmpty(text))
                sb.Append(text).Append(' ');
        }

        if (!root.TryGetProperty("content", out var contentProp) || contentProp.ValueKind != JsonValueKind.Array)
            return;

        foreach (var child in contentProp.EnumerateArray())
            ExtractText(child, sb);

        sb.Append('\n');
    }

    public static string GenerateSlug(string title)
    {
        if (string.IsNullOrWhiteSpace(title))
            return string.Empty;

        var slug = title
            .Trim()
            .ToLowerInvariant()
            .Replace("ı", "i")
            .Replace("ğ", "g")
            .Replace("ü", "u")
            .Replace("ş", "s")
            .Replace("ö", "o")
            .Replace("ç", "c")
            .Replace("&", " and ");

        slug = SlugRegex().Replace(slug, "-");

        return slug.Trim('-');
    }

    public static string GetPlainTextFromJsonContent(string jsonContent)
    {
        if (string.IsNullOrEmpty(jsonContent))
            return string.Empty;

        try
        {
            using var document = JsonDocument.Parse(jsonContent);

            var sb = new StringBuilder();
            ExtractText(document.RootElement, sb);
            return sb.ToString().Trim();
        }
        catch (JsonException)
        {
            return jsonContent;
        }
    }

    [GeneratedRegex(@"[^a-z0-9]+")]
    private static partial Regex SlugRegex();
}