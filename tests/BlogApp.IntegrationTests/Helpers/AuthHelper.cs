using System.Net.Http.Json;
using BlogApp.Api.Endpoints.Auth.Requests;
using BlogApp.Domain.Models.Auth;

namespace BlogApp.IntegrationTests.Helpers;

public static class AuthHelper
{
    public static async Task<LoginResult?> LoginAsync(HttpClient httpClient, string email, string password)
    {
        var request = new LoginRequest(email, password);
        var response = await httpClient.PostAsJsonAsync("/api/v1/auth/login", request);
        return await response.Content.ReadFromJsonAsync<LoginResult>();
    }

    public static async Task RegisterAsync(HttpClient httpClient, string email = "john@example.com",
        string password = "Password123!")
    {
        var request = new RegisterRequest(
            FirstName: "John",
            LastName: "Doe",
            Email: email,
            Username: email.Split('@')[0],
            Password: password,
            ConfirmedPassword: password);

        await httpClient.PostAsJsonAsync("/api/v1/auth/register", request);
    }
}