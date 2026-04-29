using BlogApp.Domain.Models.Auth;

namespace BlogApp.Application.Auth.Commands;

public record LoginCommand(string Email, string Password) : LoginArgs(Email, Password), IRequest<Result<LoginResult>>;