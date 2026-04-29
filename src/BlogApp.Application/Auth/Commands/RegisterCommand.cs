using BlogApp.Domain.Models.Auth;

namespace BlogApp.Application.Auth.Commands;

public record RegisterCommand(
    string FirstName,
    string LastName,
    string Email,
    string Username,
    string Password,
    string ConfirmedPassword)
    : RegisterArgs(FirstName, LastName, Email, Username, Password),
        IRequest<Result>
{
}