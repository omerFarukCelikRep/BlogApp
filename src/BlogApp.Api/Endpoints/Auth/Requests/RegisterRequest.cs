using BlogApp.Application.Auth.Commands;
using BlogApp.Core.Security.Attributes;

namespace BlogApp.Api.Endpoints.Auth.Requests;

public record RegisterRequest(
    [Sanitize]string FirstName,
    [Sanitize]string LastName,
    [Sanitize]string Email,
    [Sanitize]string Username,
    string Password,
    string ConfirmedPassword)
{
    public static explicit operator RegisterCommand(RegisterRequest request) => new(request.FirstName, request.LastName,
        request.Email, request.Username, request.Password, request.ConfirmedPassword);
}