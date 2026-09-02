using BlogApp.Application.Auth.Commands;

namespace BlogApp.Api.Endpoints.Auth.Requests;

public record RefreshTokenRequest(string Token)
{
    public static explicit operator RefreshTokenCommand(RefreshTokenRequest request) => new(request.Token);
}