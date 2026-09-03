using System.Net;
using System.Net.Http.Json;
using BlogApp.IntegrationTests.Fixtures;
using BlogApp.IntegrationTests.Helpers;
using FluentAssertions;

namespace BlogApp.IntegrationTests.Auth;

[Collection("Integration")]
public class RegisterEndpointTests(BlogAppFactory factory) :IClassFixture<DatabaseFixture> 
{
    private const string RegisterEndpoint = "api/v1/auth/register";
    
    private readonly HttpClient _client = factory.CreateClient();
    private readonly DatabaseFixture _db = new(factory);

    [Fact]
    public async Task Register_ValidRequest_Returns201()
    {
        var request = FakeDataBuilder.ValidRegisterRequest();
        
        var response = await _client.PostAsJsonAsync(RegisterEndpoint, request, cancellationToken: TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }
    
    [Fact]
    public async Task Register_DuplicateEmail_Returns400()
    {
        const string email = "duplicate@example.com";
        var request = FakeDataBuilder.ValidRegisterRequest(email);
        
        await _client.PostAsJsonAsync(RegisterEndpoint,request,TestContext.Current.CancellationToken);
        
        var response = await _client.PostAsJsonAsync(RegisterEndpoint,request,TestContext.Current.CancellationToken);
        
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Register_PasswordMismatch_Returns400()
    {
        var request = FakeDataBuilder.ValidRegisterRequest() with
        {
            ConfirmedPassword = "DifferentPassword"
        };
        
        var response = await _client.PostAsJsonAsync(RegisterEndpoint, request,TestContext.Current.CancellationToken);
        
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
    
    [Fact]
    public async Task Register_InvalidEmail_Returns400()
    {
        var request = FakeDataBuilder.ValidRegisterRequest("notanemail");
        
        var response = await _client.PostAsJsonAsync(RegisterEndpoint, request,TestContext.Current.CancellationToken);
        
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Theory]
    [InlineData("", "Doe", "test@example.com", "testuser", "Password123!", "Password123!")]
    [InlineData("John", "", "test@example.com", "testuser", "Password123!", "Password123!")]
    [InlineData("John", "Doe", "", "testuser", "Password123!", "Password123!")]
    [InlineData("John", "Doe", "test@example.com", "", "Password123!", "Password123!")]
    [InlineData("John", "Doe", "test@example.com", "testuser", "", "")]
    public async Task Register_MissingRequiredField_Returns400(string firstName, string lastName, string email,
        string username, string password, string confirmPassword)
    {
        var request = new
        {
            FirstName = firstName,
            LastName = lastName,
            Email = email,
            Username = username,
            Password = password,
            ConfirmedPassword = confirmPassword
        };
        
        var response = await _client.PostAsJsonAsync(RegisterEndpoint, request,TestContext.Current.CancellationToken);
        
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Register_PasswordTooShort_Returns400()
    {
        var request = FakeDataBuilder.ValidRegisterRequest(password:"short");
        
        var response = await _client.PostAsJsonAsync(RegisterEndpoint, request,TestContext.Current.CancellationToken);
        
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
    
    [Fact]
    public async Task Register_ValidRequest_CanLoginAfterwards()
    {
        const string email = "register_then_logic@example.com";
        const string password = "Password123!";
        var request = FakeDataBuilder.ValidRegisterRequest(email, password);
        
        await _client.PostAsJsonAsync(RegisterEndpoint, request,TestContext.Current.CancellationToken);

        var loginResult = await AuthHelper.LoginAsync(_client, email, password);

        loginResult.Should().NotBeNull();
        loginResult.Token.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task Register_ValidRequest_ReturnsJsonContentType()
    {
        var request = FakeDataBuilder.ValidRegisterRequest();
        
        var response = await _client.PostAsJsonAsync(RegisterEndpoint, request,TestContext.Current.CancellationToken);
        
        response.Content.Headers.ContentType?.MediaType.Should().Be("application/json");
    }
}