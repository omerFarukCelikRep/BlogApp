using BlogApp.Application.Auth.Commands;

namespace BlogApp.Api.Endpoints.Auth.Requests;

public class RefreshTokenRequest
{
    public string Token { get; set; } = string.Empty;

    public static explicit operator RefreshTokenCommand(RefreshTokenRequest request) => new()
    {
        Token = request.Token
    };
}