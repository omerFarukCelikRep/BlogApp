using BlogApp.Application.Auth.Commands;
using FluentAssertions;

namespace BlogApp.UnitTests.Core.Validations;

public class RegisterCommandValidatorTests
{
    private readonly RegisterCommandValidator _validator = new();

    private static RegisterCommand ValidCommand() => new(
        FirstName: "John",
        LastName: "Doe",
        Email: "john@example.com",
        Username: "johndoe",
        Password: "Password123!",
        ConfirmedPassword: "Password123!");

    [Fact]
    public async Task ValidateAsync_ValidCommand_ShouldPass()
    {
        var result = await _validator.ValidateAsync(ValidCommand(), TestContext.Current.CancellationToken);

        result.IsValid.Should().BeTrue();
        result.Errors.Should().BeEmpty();
    }

    // FirstName Rules
    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public async Task ValidateAsync_EmptyFirstName_ShouldFail(string? firstName)
    {
        var command = ValidCommand() with { FirstName = firstName! };

        var result = await _validator.ValidateAsync(command, TestContext.Current.CancellationToken);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(RegisterCommand.FirstName));
    }

    [Fact]
    public async Task ValidateAsync_FirstNameTooShort_ShouldFail()
    {
        var command = ValidCommand() with { FirstName = "J" };

        var result = await _validator.ValidateAsync(command, TestContext.Current.CancellationToken);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(RegisterCommand.FirstName));
    }

    // LastName Rules
    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public async Task ValidateAsync_EmptyLastName_ShouldFail(string? lastName)
    {
        var command = ValidCommand() with { LastName = lastName! };

        var result = await _validator.ValidateAsync(command, TestContext.Current.CancellationToken);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(RegisterCommand.LastName));
    }

    // Email Rules
    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public async Task ValidateAsync_EmptyEmail_ShouldFail(string? email)
    {
        var command = ValidCommand() with { Email = email! };

        var result = await _validator.ValidateAsync(command, TestContext.Current.CancellationToken);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(RegisterCommand.Email));
    }

    [Theory]
    [InlineData("notanemail")]
    [InlineData("missing@")]
    [InlineData("@nodomain.com")]
    public async Task ValidateAsync_InvalidEmailFormat_ShouldFail(string email)
    {
        var command = ValidCommand() with { Email = email };

        var result = await _validator.ValidateAsync(command, TestContext.Current.CancellationToken);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(RegisterCommand.Email));
    }

    // Username Rules
    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public async Task ValidateAsync_EmptyUsername_ShouldFail(string? username)
    {
        var command = ValidCommand() with { Username = username! };

        var result = await _validator.ValidateAsync(command, TestContext.Current.CancellationToken);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(RegisterCommand.Username));
    }

    [Fact]
    public async Task ValidateAsync_UsernameTooShort_ShouldFail()
    {
        var command = ValidCommand() with { Username = "ab" };

        var result = await _validator.ValidateAsync(command, TestContext.Current.CancellationToken);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(RegisterCommand.Username));
    }

    // Password Rules
    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public async Task ValidateAsync_EmptyPassword_ShouldFail(string? password)
    {
        var command = ValidCommand() with { Password = password! };

        var result = await _validator.ValidateAsync(command, TestContext.Current.CancellationToken);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(RegisterCommand.Password));
    }

    [Fact]
    public async Task ValidateAsync_PasswordTooShort_ShouldFail()
    {
        var command = ValidCommand() with { Password = "short", ConfirmedPassword = "short" };

        var result = await _validator.ValidateAsync(command, TestContext.Current.CancellationToken);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(RegisterCommand.Password));
    }

    // ConfirmedPassword Rules
    [Fact]
    public async Task ValidateAsync_PasswordMismatch_ShouldFail()
    {
        var command = ValidCommand() with { ConfirmedPassword = "DifferentPassword!" };

        var result = await _validator.ValidateAsync(command, TestContext.Current.CancellationToken);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(RegisterCommand.ConfirmedPassword));
    }

    [Fact]
    public async Task ValidateAsync_EmptyConfirmedPassword_ShouldFail()
    {
        var command = ValidCommand() with { ConfirmedPassword = "" };

        var result = await _validator.ValidateAsync(command, TestContext.Current.CancellationToken);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(RegisterCommand.ConfirmedPassword));
    }

    // Multiple Errors
    [Fact]
    public async Task ValidateAsync_MultipleInvalidFields_ShouldReturnMultipleErrors()
    {
        var command = new RegisterCommand("", "", "", "", "", "");

        var result = await _validator.ValidateAsync(command, TestContext.Current.CancellationToken);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().HaveCountGreaterThan(3);
    }
}