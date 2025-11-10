using BlogApp.Core.Results;
using BlogApp.Domain.Abstractions.Services;
using BlogApp.Domain.Models.Auth;

namespace BlogApp.Application.Auth.Commands;

public class LoginCommandHandler(IAuthenticationService authenticationService)
    : IRequestHandler<LoginCommand, Result<LoginResult>>
{
    public async Task<Result<LoginResult>> Handle(LoginCommand request, CancellationToken cancellationToken = default)
    {
        return await authenticationService.LoginAsync(request, cancellationToken);
    }
}