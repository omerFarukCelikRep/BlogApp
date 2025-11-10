using BlogApp.Domain.Models.Auth;

namespace BlogApp.Application.Auth.Commands;

public class RegisterCommand : RegisterArgs, IRequest<Result>
{
}