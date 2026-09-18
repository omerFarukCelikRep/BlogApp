using BlogApp.Domain.Abstractions.Services;
using BlogApp.Domain.Models.Auth;

namespace BlogApp.Application.Auth.Commands;

public class Verify2FACommandHandler(IAuthenticationService authenticationService) : IRequestHandler<Verify2FACommand,Result<LoginResult>>
{
    public async Task<Result<LoginResult>> Handle(Verify2FACommand request, CancellationToken cancellationToken = default)
    {
        return await authenticationService.Verify2FAOtpAsync(request, cancellationToken);
    }
}