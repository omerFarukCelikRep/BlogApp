using BlogApp.Application.Blogs.Commands;
using FluentAssertions;

namespace BlogApp.UnitTests.Core.Validations.Blogs;

public class CreateBlogCommandValidatorTests
{
    private readonly CreateBlogCommandValidator _validator = new();

    private static CreateBlogCommand ValidCommand() => new(
        Title: "My First Blog Post About .NET",
        Content: new string('x', 50),
        Thumbnail: null,
        Tags: [1, 2],
        Categories: [1]);

    [Fact]
    public async Task ValidateAsync_ValidCommand_ShouldPass()
    {
        var result = await _validator.ValidateAsync(ValidCommand(), TestContext.Current.CancellationToken);

        result.IsValid.Should().BeTrue();
        result.Errors.Should().BeEmpty();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public async Task ValidateAsync_EmptyTitle_ShouldFail(string? title)
    {
        var command = ValidCommand() with { Title = title! };
        var result = await _validator.ValidateAsync(command, TestContext.Current.CancellationToken);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(CreateBlogCommand.Title));
    }

    [Fact]
    public async Task ValidateAsync_TitleTooShort_ShouldFail()
    {
        var command = ValidCommand() with { Title = "Hi" };
        var result = await _validator.ValidateAsync(command, TestContext.Current.CancellationToken);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e =>
            e.PropertyName == nameof(CreateBlogCommand.Title));
    }

    [Fact]
    public async Task ValidateAsync_TitleTooLong_ShouldFail()
    {
        var command = ValidCommand() with { Title = new string('x', 513) };
        var result = await _validator.ValidateAsync(command, TestContext.Current.CancellationToken);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e =>
            e.PropertyName == nameof(CreateBlogCommand.Title));
    }

    [Fact]
    public async Task ValidateAsync_ContentTooShort_ShouldFail()
    {
        var command = ValidCommand() with { Content = "Too short" };
        var result = await _validator.ValidateAsync(command, TestContext.Current.CancellationToken);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e =>
            e.PropertyName == nameof(CreateBlogCommand.Content));
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public async Task ValidateAsync_EmptyContent_ShouldFail(string? content)
    {
        var command = ValidCommand() with { Content = content! };
        var result = await _validator.ValidateAsync(command, TestContext.Current.CancellationToken);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e =>
            e.PropertyName == nameof(CreateBlogCommand.Content));
    }

    [Fact]
    public async Task ValidateAsync_EmptyTagIds_ShouldFail()
    {
        var command = ValidCommand() with { Tags = [] };
        var result = await _validator.ValidateAsync(command, TestContext.Current.CancellationToken);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e =>
            e.PropertyName == nameof(CreateBlogCommand.Tags));
    }

    [Fact]
    public async Task ValidateAsync_TooManyTagIds_ShouldFail()
    {
        var command = ValidCommand() with
        {
            Tags = [.. Enumerable.Range(1, 11)]
        };
        var result = await _validator.ValidateAsync(command, TestContext.Current.CancellationToken);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e =>
            e.PropertyName == nameof(CreateBlogCommand.Tags));
    }

    [Fact]
    public async Task ValidateAsync_EmptyCategoryIds_ShouldFail()
    {
        var command = ValidCommand() with { Categories = [] };
        var result = await _validator.ValidateAsync(command, TestContext.Current.CancellationToken);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e =>
            e.PropertyName == nameof(CreateBlogCommand.Categories));
    }

    [Fact]
    public async Task ValidateAsync_TooManyCategoryIds_ShouldFail()
    {
        var command = ValidCommand() with
        {
            Categories = [.. Enumerable.Range(1, 6)]
        };
        var result = await _validator.ValidateAsync(command, TestContext.Current.CancellationToken);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e =>
            e.PropertyName == nameof(CreateBlogCommand.Categories));
    }

    [Fact]
    public async Task ValidateAsync_NullThumbnail_ShouldPass()
    {
        var command = ValidCommand() with { Thumbnail = null };
        var result = await _validator.ValidateAsync(command, TestContext.Current.CancellationToken);

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public async Task ValidateAsync_ThumbnailTooLong_ShouldFail()
    {
        var command = ValidCommand() with
        {
            Thumbnail = new string('x', 1025)
        };
        var result = await _validator.ValidateAsync(command, TestContext.Current.CancellationToken);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e =>
            e.PropertyName == nameof(CreateBlogCommand.Thumbnail));
    }
}