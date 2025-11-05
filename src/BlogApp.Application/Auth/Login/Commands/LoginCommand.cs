using BlogApp.Core.Results;
using BlogApp.Domain.Models.Auth;

namespace BlogApp.Application.Auth.Login.Commands;

public class LoginCommand : LoginArgs, IRequest<Result<LoginResult>>
{
}