using BlogApp.Domain.Abstractions.Services;

namespace BlogApp.Application.Auth.Commands;

public class ConfirmDisable2FACommandHandler(IAuthenticationService authenticationService)
    : IRequestHandler<ConfirmDisable2FACommand, Result>
{
    public async Task<Result> Handle(ConfirmDisable2FACommand request, CancellationToken cancellationToken = default)
    {
        return await authenticationService.ConfirmDisable2FAAsync(request, cancellationToken);
    }
}