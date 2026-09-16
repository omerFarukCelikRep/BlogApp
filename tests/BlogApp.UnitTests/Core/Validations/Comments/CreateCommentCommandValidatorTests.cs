using BlogApp.Application.Comments.Commands;
using FluentAssertions;

namespace BlogApp.UnitTests.Core.Validations.Comments;

public class CreateCommentCommandValidatorTests
{
    private readonly CreateCommentCommandValidator _validator = new();

    private static CreateCommentCommand ValidCommand() => new(
        BlogId: 1,
        Content: "This is a great blog post!",
        ParentId: null);

    [Fact]
    public async Task ValidateAsync_ValidCommand_ShouldPass()
    {
        var result = await _validator.ValidateAsync(ValidCommand(), TestContext.Current.CancellationToken);
        result.IsValid.Should().BeTrue();
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
            e.PropertyName == nameof(CreateCommentCommand.Content));
    }

    [Fact]
    public async Task ValidateAsync_ContentTooShort_ShouldFail()
    {
        var command = ValidCommand() with { Content = "x" };
        var result = await _validator.ValidateAsync(command, TestContext.Current.CancellationToken);

        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public async Task ValidateAsync_ContentTooLong_ShouldFail()
    {
        var command = ValidCommand() with { Content = new string('x', 2001) };
        var result = await _validator.ValidateAsync(command, TestContext.Current.CancellationToken);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e =>
            e.PropertyName == nameof(CreateCommentCommand.Content));
    }

    [Fact]
    public async Task ValidateAsync_ValidReply_ShouldPass()
    {
        var command = ValidCommand() with { ParentId = 5 };
        var result = await _validator.ValidateAsync(command, TestContext.Current.CancellationToken);

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public async Task ValidateAsync_InvalidBlogId_ShouldFail()
    {
        var command = ValidCommand() with { BlogId = 0 };
        var result = await _validator.ValidateAsync(command, TestContext.Current.CancellationToken);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e =>
            e.PropertyName == nameof(CreateCommentCommand.BlogId));
    }
}