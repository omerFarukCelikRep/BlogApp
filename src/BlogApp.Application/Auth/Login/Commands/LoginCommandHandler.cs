using BlogApp.Core.Results;
using BlogApp.Domain.Abstractions.Services;
using BlogApp.Domain.Models.Auth;
using BlogApp.Domain.Services;

namespace BlogApp.Application.Auth.Login.Commands;

public class LoginCommandHandler(IAuthService authService)
    : IRequestHandler<LoginCommand, Result<LoginResult>>
{
    public async Task<Result<LoginResult>> Handle(LoginCommand request, CancellationToken cancellationToken = default)
    {
        return await authService.LoginAsync(request, cancellationToken);
    }
}