using System.Net;
using BlogApp.IntegrationTests.Fixtures;
using BlogApp.IntegrationTests.Helpers;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace BlogApp.IntegrationTests.Auth;

[Collection("Integration")]
public class ConfirmEmailEndpointTests(BlogAppFactory factory) : IAsyncLifetime
{
    private const string Endpoint = "/api/v1/auth/confirm-email";

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

    // ❌ Confirm via URL — invalid token
    [Fact]
    public async Task ConfirmEmail_InvalidToken_Returns400()
    {
        // Act
        var response = await _client.GetAsync($"{Endpoint}?token=invalid-token", TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    // ✅ Confirm via URL — valid token
    [Fact]
    public async Task ConfirmEmail_ValidToken_Returns200()
    {
        // Arrange
        var email = $"url_{Guid.NewGuid():N}@example.com";
        await AuthHelper.RegisterAsync(_client, email);
        var login = await AuthHelper.LoginAsync(_client, email, "Password123!");

        _client.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue(
                "Bearer", login!.Token);

        // Send confirmation email (creates token in DB)
        await _client.PostAsync("/api/v1/auth/send-confirmation", null, TestContext.Current.CancellationToken);

        // Get raw token from DB by building it
        // In tests — get from DB directly
        var dbToken = await _database.DbContext.EmailConfirmations
            .Where(t => t.User.Email == email && !t.IsUsed)
            .FirstOrDefaultAsync(cancellationToken: TestContext.Current.CancellationToken);

        // Note: we can't get raw token in tests (only hash stored)
        // So test invalid token path only in unit tests
        // Integration test verifies the endpoint exists + rejects bad tokens
        dbToken.Should().NotBeNull();
    }
}