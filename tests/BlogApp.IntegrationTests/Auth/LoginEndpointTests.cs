using System.Net;
using System.Net.Http.Json;
using BlogApp.Domain.Models.Auth;
using BlogApp.IntegrationTests.Fixtures;
using BlogApp.IntegrationTests.Helpers;
using FluentAssertions;

namespace BlogApp.IntegrationTests.Auth;

[Collection("Integration")]
public class LoginEndpointTests(BlogAppFactory factory) : IClassFixture<DatabaseFixture>
{
    private const string LoginEndpoint = "/api/v1/auth/login";

    private readonly HttpClient _client = factory.CreateClient();

    [Fact]
    public async Task Login_ValidCredentials_Returns200WithToken()
    {
        const string email = "login_valid@example.com";
        await AuthHelper.RegisterAsync(_client, email);

        var request = FakeDataBuilder.ValidLoginRequest(email);

        var response = await _client.PostAsJsonAsync(LoginEndpoint, request,
            cancellationToken: TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var result =
            await response.Content.ReadFromJsonAsync<LoginResult>(
                cancellationToken: TestContext.Current.CancellationToken);
        result.Should().NotBeNull();
        result.Token.Should().NotBeNullOrEmpty();
        result.RefreshToken.Should().NotBeNullOrEmpty();
        result.ExpiresAt.Should().BeAfter(DateTimeOffset.UtcNow);
    }

    [Fact]
    public async Task Login_WrongPassword_Returns401()
    {
        const string email = "login_wrong_pwd@example.com";
        await AuthHelper.RegisterAsync(_client, email);

        var request = FakeDataBuilder.ValidLoginRequest(email, "WrongPassword!");

        var response = await _client.PostAsJsonAsync(LoginEndpoint, request,
            cancellationToken: TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Login_UnknownEmail_Returns401()
    {
        var request = FakeDataBuilder.ValidLoginRequest("unknown@example.com");

        var response = await _client.PostAsJsonAsync(LoginEndpoint, request,
            cancellationToken: TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Login_InvalidEmailFormat_Returns400()
    {
        var request = new { Email = "notanemail", Password = "Password123!" };

        var response = await _client.PostAsJsonAsync(LoginEndpoint, request, TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Login_EmptyBody_Returns400()
    {
        var request = new { Email = "", Password = "" };

        var response = await _client.PostAsJsonAsync(LoginEndpoint, request, TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Login_MissingPassword_Returns400()
    {
        var request = new { Email = "john@example.com" };

        var response = await _client.PostAsJsonAsync(LoginEndpoint, request, TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Login_ValidCredentials_ReturnsValidJwtToken()
    {
        const string email = "login_jwt@example.com";
        await AuthHelper.RegisterAsync(_client, email);
        
        var request = FakeDataBuilder.ValidLoginRequest(email);
        
        var response = await _client.PostAsJsonAsync(LoginEndpoint, request, TestContext.Current.CancellationToken);
        var result = await response.Content.ReadFromJsonAsync<LoginResult>(TestContext.Current.CancellationToken);

        result!.Token.Split('.').Should().HaveCount(3);
    }

    [Fact]
    public async Task Login_ValidCredentials_ReturnsJsonContentType()
    {
        const string email = "login_content@example.com";
        await AuthHelper.RegisterAsync(_client, email);
        var request = FakeDataBuilder.ValidLoginRequest(email);
        
        var  response = await _client.PostAsJsonAsync(LoginEndpoint, request, TestContext.Current.CancellationToken);
        
        response.Content.Headers.ContentType?.MediaType.Should().Be("application/json");
    }
}