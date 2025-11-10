using BlogApp.Domain.Models.Auth;

namespace BlogApp.Application.Auth.Commands;

public class LoginCommand : LoginArgs, IRequest<Result<LoginResult>>
{
}