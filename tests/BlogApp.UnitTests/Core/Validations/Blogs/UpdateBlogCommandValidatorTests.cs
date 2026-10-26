using BlogApp.Application.Blogs.Commands;
using FluentAssertions;

namespace BlogApp.UnitTests.Core.Validations.Blogs;

public class UpdateBlogCommandValidatorTests
{
    private readonly UpdateBlogCommandValidator _validator = new();

    private static UpdateBlogCommand ValidCommand() => new(
        Id: 1,
        Title: "Updated Blog Post Title",
        Content: new string('x', 50),
        Thumbnail: null,
        Categories: [1],
        Tags: [1, 2]);

    [Fact]
    public async Task ValidateAsync_ValidCommand_ShouldPass()
    {
        var result = await _validator.ValidateAsync(ValidCommand(), TestContext.Current.CancellationToken);
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public async Task ValidateAsync_InvalidId_ShouldFail()
    {
        var command = ValidCommand() with { Id = 0 };
        var result = await _validator.ValidateAsync(command, TestContext.Current.CancellationToken);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(UpdateBlogCommand.Id));
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public async Task ValidateAsync_EmptyTitle_ShouldFail(string? title)
    {
        var command = ValidCommand() with { Title = title! };
        var result = await _validator.ValidateAsync(command, TestContext.Current.CancellationToken);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(UpdateBlogCommand.Title));
    }

    [Fact]
    public async Task ValidateAsync_TitleTooShort_ShouldFail()
    {
        var command = ValidCommand() with { Title = "Hi" };
        var result = await _validator.ValidateAsync(command, TestContext.Current.CancellationToken);

        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public async Task ValidateAsync_TitleTooLong_ShouldFail()
    {
        var command = ValidCommand() with { Title = new string('x', 513) };
        var result = await _validator.ValidateAsync(command, TestContext.Current.CancellationToken);

        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public async Task ValidateAsync_ContentTooShort_ShouldFail()
    {
        var command = ValidCommand() with { Content = "Short" };
        var result = await _validator.ValidateAsync(command, TestContext.Current.CancellationToken);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(UpdateBlogCommand.Content));
    }

    [Fact]
    public async Task ValidateAsync_EmptyTagIds_ShouldFail()
    {
        var command = ValidCommand() with { Tags = [] };
        var result = await _validator.ValidateAsync(command, TestContext.Current.CancellationToken);

        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public async Task ValidateAsync_EmptyCategoryIds_ShouldFail()
    {
        var command = ValidCommand() with { Categories = [] };
        var result = await _validator.ValidateAsync(command, TestContext.Current.CancellationToken);

        result.IsValid.Should().BeFalse();
    }
}