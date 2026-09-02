using BlogApp.Core.Security.Attributes;
using BlogApp.Core.Security.Constants;

namespace BlogApp.Application.Auth.Commands;

[Authorize]
public class LogoutCommand :  IRequest<Result>
{
}