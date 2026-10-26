using System.Net;
using BlogApp.IntegrationTests.Fixtures;
using BlogApp.IntegrationTests.Helpers;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace BlogApp.IntegrationTests.Auth;

[Collection("Integration")]
public class SendConfirmationEndpointTests(BlogAppFactory factory) : IAsyncLifetime
{
    private const string Endpoint = "/api/v1/auth/send-confirmation";

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
    public async Task SendConfirmation_AuthenticatedUser_Returns200()
    {
        // Arrange
        var email = $"user_{Guid.NewGuid():N}@example.com";
        await AuthHelper.RegisterAsync(_client, email);
        var login = await AuthHelper.LoginAsync(_client, email, "Password123!");

        _client.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue(
                "Bearer", login!.Token);

        // Act
        var response = await _client.PostAsync(Endpoint, null, TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    // ❌ Send confirmation — unauthenticated
    [Fact]
    public async Task SendConfirmation_NoAuth_Returns401()
    {
        // Arrange
        _client.DefaultRequestHeaders.Authorization = null;

        // Act
        var response = await _client.PostAsync(Endpoint, null, TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    // ❌ Already confirmed — returns 400
    [Fact]
    public async Task SendConfirmation_AlreadyConfirmed_Returns400()
    {
        // Arrange — manually confirm user
        var email = $"confirmed_{Guid.NewGuid():N}@example.com";
        await AuthHelper.RegisterAsync(_client, email);
        var login = await AuthHelper.LoginAsync(_client, email, "Password123!");

        // Confirm via DB
        var user = await _database.DbContext.Users.FirstAsync(u => u.Email == email,
            cancellationToken: TestContext.Current.CancellationToken);
        user.EmailConfirmed = true;
        await _database.DbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        _client.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue(
                "Bearer", login!.Token);

        // Act
        var response = await _client.PostAsync(Endpoint, null, TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}