using BlogApp.Application.Auth.Commands;

namespace BlogApp.Api.Endpoints.Auth.Requests;

public record LoginRequest(string Email, string Password)
{
    public static explicit operator LoginCommand(LoginRequest request) => new(request.Email, request.Password);
}