using BlogApp.Api.Endpoints.Auth.Requests;
using Bogus;

namespace BlogApp.IntegrationTests.Helpers;

public static class FakeDataBuilder
{
    private static readonly Faker _faker = new();

    public static RegisterRequest ValidRegisterRequest(string? email = null, string? password = null)
    {
        var pwd = password ?? "Password123!";
        return new RegisterRequest(
            FirstName: _faker.Name.FirstName(),
            LastName: _faker.Name.LastName(),
            Email: email ?? _faker.Internet.Email(),
            Username: _faker.Internet.UserName(),
            Password: pwd,
            ConfirmedPassword: pwd);
    }

    public static LoginRequest ValidLoginRequest(string email, string password = "Password123!") =>
        new(email, password);

    public static LoginRequest InvalidLoginRequest() =>
        new(_faker.Internet.Email(), "WrongPassword!");
}