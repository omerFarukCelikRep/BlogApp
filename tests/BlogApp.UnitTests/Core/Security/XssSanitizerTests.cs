using BlogApp.Core.Security.Attributes;
using BlogApp.Core.Security.Options;
using BlogApp.Core.Security.Sanitizers;
using FluentAssertions;
using Microsoft.Extensions.Options;

namespace BlogApp.UnitTests.Core.Security;

public class XssSanitizerTests
{
    private readonly XssSanitizer _sanitizer;

    public XssSanitizerTests()
    {
        var options = Options.Create(new XssOptions());
        _sanitizer = new XssSanitizer(options);
    }

    // StripHtml
    [Theory]
    [InlineData("<script>alert('xss')</script>Hello", "Hello")]
    [InlineData("<p>Hello</p>", "Hello")]
    [InlineData("<b>Bold</b> text", "Bold text")]
    [InlineData("Plain text", "Plain text")]
    [InlineData("", "")]
    [InlineData("<img src=x onerror=alert('xss')>", "")]
    [InlineData("<a href='javascript:alert(1)'>click</a>", "click")]
    public void StripHtml_RemovesAllHtmlTags(string input, string expected)
    {
        var result = _sanitizer.StripHtml(input);
        result.Should().Be(expected);
    }

    [Fact]
    public void StripHtml_RemovesScriptWithContent()
    {
        var input = "<script>var x = steal(document.cookie);</script>Hello";
        var result = _sanitizer.StripHtml(input);

        result.Should().Be("Hello");
        result.Should().NotContain("steal");
        result.Should().NotContain("document.cookie");
    }

    [Fact]
    public void StripHtml_RemovesStyleWithContent()
    {
        var input = "<style>body { background: url(evil) }</style>Hello";
        var result = _sanitizer.StripHtml(input);

        result.Should().Be("Hello");
        result.Should().NotContain("background");
    }

    [Fact]
    public void StripHtml_RemovesHtmlComments()
    {
        var input = "Hello <!-- <script>alert('xss')</script> --> World";
        var result = _sanitizer.StripHtml(input);

        result.Should().Be("Hello  World");
        result.Should().NotContain("<script>");
    }

    // SanitizeHtml
    [Theory]
    [InlineData("<p>Hello <strong>World</strong></p>", "<p>Hello <strong>World</strong></p>")]
    [InlineData("<ul><li>Item 1</li><li>Item 2</li></ul>", "<ul><li>Item 1</li><li>Item 2</li></ul>")]
    [InlineData("<code>var x = 1;</code>", "<code>var x = 1;</code>")]
    [InlineData("<blockquote>Quote</blockquote>", "<blockquote>Quote</blockquote>")]
    public void SanitizeHtml_PreservesSafeHtml(string input, string expected)
    {
        var result = _sanitizer.SanitizeHtml(input);
        result.Should().Be(expected);
    }

    [Theory]
    [InlineData("<script>alert('xss')</script><p>Safe</p>")]
    [InlineData("<p onclick=\"alert('xss')\">text</p>")]
    [InlineData("<img src=\"x\" onerror=\"alert('xss')\">")]
    [InlineData("<iframe src=\"https://evil.com\"></iframe>")]
    [InlineData("<a href=\"javascript:alert(1)\">click</a>")]
    public void SanitizeHtml_RemovesDangerousContent(string input)
    {
        var result = _sanitizer.SanitizeHtml(input);

        result.Should().NotContain("<script>");
        result.Should().NotContain("alert(");
        result.Should().NotContain("onerror");
        result.Should().NotContain("onclick");
        result.Should().NotContain("<iframe>");
        result.Should().NotContain("javascript:");
    }

    [Fact]
    public void SanitizeHtml_RemovesEventHandlers()
    {
        var input = "<div onclick=\"steal()\" onmouseover=\"evil()\">text</div>";
        var result = _sanitizer.SanitizeHtml(input);

        result.Should().NotContain("onclick");
        result.Should().NotContain("onmouseover");
        result.Should().NotContain("steal()");
        result.Should().NotContain("evil()");
        result.Should().Contain("text");
    }

    [Fact]
    public void SanitizeHtml_RemovesJavascriptUrls()
    {
        var input = "<a href=\"javascript:alert('xss')\">click</a>";
        var result = _sanitizer.SanitizeHtml(input);

        result.Should().NotContain("javascript:");
        result.Should().Contain("click");
    }

    [Fact]
    public void SanitizeHtml_RemovesDataUrls()
    {
        var input = "<img src=\"data:text/html,<script>alert('xss')</script>\">";
        var result = _sanitizer.SanitizeHtml(input);

        result.Should().NotContain("data:");
        result.Should().NotContain("<script>");
    }

    // HtmlEncode
    [Theory]
    [InlineData("<", "&lt;")]
    [InlineData(">", "&gt;")]
    [InlineData("&", "&amp;")]
    [InlineData("\"", "&quot;")]
    [InlineData("'", "&#x27;")]
    [InlineData("Plain text", "Plain text")]
    public void HtmlEncode_EncodesSpecialCharacters(
        string input, string expected)
    {
        var result = _sanitizer.HtmlEncode(input);
        result.Should().Be(expected);
    }

    // Sanitize — via SanitizeAttribute
    [Fact]
    public void Sanitize_AllowHtmlFalse_StripsAllHtml()
    {
        var attr = new SanitizeAttribute { AllowHtml = false };
        var input = "<p>Hello <script>alert('xss')</script></p>";
        var result = _sanitizer.Sanitize(input, attr);

        result.Should().Be("Hello");
        result.Should().NotContain("<p>");
        result.Should().NotContain("<script>");
    }

    [Fact]
    public void Sanitize_AllowHtmlTrue_KeepsSafeHtml()
    {
        var attr = new SanitizeAttribute { AllowHtml = true };
        var input = "<p>Hello <strong>World</strong></p>";
        var result = _sanitizer.Sanitize(input, attr);

        result.Should().Contain("<p>");
        result.Should().Contain("<strong>World</strong>");
    }

    [Fact]
    public void Sanitize_AllowHtmlTrue_StillRemovesScript()
    {
        var attr = new SanitizeAttribute { AllowHtml = true };
        var input = "<p>Safe</p><script>alert('xss')</script>";
        var result = _sanitizer.Sanitize(input, attr);

        result.Should().Contain("<p>Safe</p>");
        result.Should().NotContain("<script>");
        result.Should().NotContain("alert('xss')");
    }

    [Fact]
    public void Sanitize_WithMaxLength_TruncatesResult()
    {
        var attr = new SanitizeAttribute { MaxLength = 10 };
        var input = "Hello World this is a long string";
        var result = _sanitizer.Sanitize(input, attr);

        result.Length.Should().BeLessThanOrEqualTo(10);
        result.Should().Be("Hello Worl");
    }

    [Fact]
    public void Sanitize_HtmlEncodeTrue_EncodesResult()
    {
        var attr = new SanitizeAttribute { HtmlEncode = true };
        var input = "Hello <World>";
        var result = _sanitizer.Sanitize(input, attr);

        result.Should().Contain("&lt;");
        result.Should().Contain("&gt;");
        result.Should().NotContain("<World>");
    }

    // NULL + EMPTY HANDLING
    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public void StripHtml_NullOrEmpty_ReturnsInput(string? input)
    {
        var result = _sanitizer.StripHtml(input!);
        result.Should().Be(input);
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public void SanitizeHtml_NullOrEmpty_ReturnsInput(string? input)
    {
        var result = _sanitizer.SanitizeHtml(input!);
        result.Should().Be(input);
    }

    // BLOG SPECIFIC SCENARIOS
    [Fact]
    public void SanitizeHtml_BlogContent_WithCodeAndScript_CodePreserved()
    {
        var input = """
                    <h2>My Blog Post</h2>
                    <p>Here is some <strong>code</strong>:</p>
                    <pre><code>var x = 1;</code></pre>
                    <script>alert('xss')</script>
                    <ul><li>Safe item</li></ul>
                    """;

        var result = _sanitizer.SanitizeHtml(input);

        result.Should().Contain("<h2>My Blog Post</h2>");
        result.Should().Contain("<pre><code>var x = 1;</code></pre>");
        result.Should().Contain("<ul><li>Safe item</li></ul>");
        result.Should().NotContain("<script>");
        result.Should().NotContain("alert('xss')");
    }

    [Fact]
    public void StripHtml_RegisterForm_NameFields_Clean()
    {
        var inputs = new[]
        {
            ("<script>alert('xss')</script>John", "John"),
            ("<b>Jane</b>", "Jane"),
            ("Normal Name", "Normal Name"),
        };

        foreach (var (input, expected) in inputs)
        {
            var result = _sanitizer.StripHtml(input);
            result.Should().Be(expected);
        }
    }
}