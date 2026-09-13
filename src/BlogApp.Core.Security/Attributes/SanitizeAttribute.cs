namespace BlogApp.Core.Security.Attributes;

/// <summary>
/// Marks a string property for XSS sanitization
/// in the XssEndpointFilter pipeline.
/// </summary>
[AttributeUsage(
    AttributeTargets.Property | AttributeTargets.Parameter)]
public sealed class SanitizeAttribute : Attribute
{
    /// <summary>
    /// When true — allow safe HTML tags from XssOptions.AllowedTags.
    /// When false (default) — strip ALL html, plain text only.
    /// </summary>
    public bool AllowHtml { get; init; } = false;

    /// <summary>
    /// When true — also encode special characters (&, <, >, ", ').
    /// </summary>
    public bool HtmlEncode { get; init; } = false;

    /// <summary>
    /// Max allowed length after sanitization.
    /// 0 = no limit.
    /// </summary>
    public int MaxLength { get; init; } = 0;

    public SanitizeAttribute() { }

    public SanitizeAttribute(bool allowHtml)
    {
        AllowHtml = allowHtml;
    }
}