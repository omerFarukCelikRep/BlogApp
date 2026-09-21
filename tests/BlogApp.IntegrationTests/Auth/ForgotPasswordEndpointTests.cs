using System.Net;
using System.Net.Http.Json;
using BlogApp.IntegrationTests.Fixtures;
using BlogApp.IntegrationTests.Helpers;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace BlogApp.IntegrationTests.Auth;

[Collection("Integration")]
public class ForgotPasswordEndpointTests(BlogAppFactory factory) : IAsyncLifetime
{
    private const string Endpoint = "/api/v1/auth/forgot-password";

    private readonly HttpClient _client = factory.CreateClient();
    private readonly DatabaseFixture _database = new(factory);

    public async ValueTask DisposeAsync()
    {
        await factory.DisposeAsync();
        GC.SuppressFinalize(this);
    }

    public async ValueTask InitializeAsync()
    {
        await _database.InitializeAsync();
    }

    [Fact]
    public async Task ForgotPassword_ExistingEmail_Returns200()
    {
        // Arrange
        var email = $"user_{Guid.NewGuid():N}@example.com";
        await AuthHelper.RegisterAsync(_client, email);

        // Act
        var response = await _client.PostAsJsonAsync(Endpoint, new { Email = email },
            cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task ForgotPassword_NonExistentEmail_StillReturns200()
    {
        // Act — email doesn't exist
        var response = await _client.PostAsJsonAsync(Endpoint, new { Email = "nonexistent@example.com" },
            cancellationToken: TestContext.Current.CancellationToken);

        // Assert — never reveals if email exists
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task ForgotPassword_CreatesTokenInDatabase()
    {
        // Arrange
        var email = $"user_{Guid.NewGuid():N}@example.com";
        await AuthHelper.RegisterAsync(_client, email);

        // Act
        await _client.PostAsJsonAsync(Endpoint, new { Email = email },
            cancellationToken: TestContext.Current.CancellationToken);

        // Assert — token created in DB
        var user = await _database.DbContext.Users.FirstAsync(u => u.Email == email,
            cancellationToken: TestContext.Current.CancellationToken);

        var token = await _database.DbContext.PasswordResetTokens.FirstOrDefaultAsync(
            t => t.UserId == user.Id && !t.IsUsed, cancellationToken: TestContext.Current.CancellationToken);

        token.Should().NotBeNull();
        token!.OtpCode.Should().HaveLength(6);
        token.ExpireDate.Should().BeAfter(DateTime.UtcNow);
    }
}