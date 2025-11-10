using BlogApp.Application.Auth.Commands;

namespace BlogApp.Api.Endpoints.Auth.Requests;

public class LoginRequest
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;

    public static explicit operator LoginCommand(LoginRequest request) => new()
    {
        Email = request.Email,
        Password = request.Password
    };
}