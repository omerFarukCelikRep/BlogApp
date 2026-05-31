using BlogApp.Application.Auth.Commands;

namespace BlogApp.Api.Endpoints.Auth.Requests;

public record RegisterRequest(
    string FirstName,
    string LastName,
    string Email,
    string Username,
    string Password,
    string ConfirmedPassword)
{
    public static explicit operator RegisterCommand(RegisterRequest request) => new(request.FirstName, request.LastName,
        request.Email, request.Username, request.Password, request.ConfirmedPassword);
}