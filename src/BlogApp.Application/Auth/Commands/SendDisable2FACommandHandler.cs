using BlogApp.Domain.Abstractions.Services;

namespace BlogApp.Application.Auth.Commands;

public class SendDisable2FACommandHandler(IAuthenticationService authenticationService) : IRequestHandler<SendDisable2FACommand,Result>
{
    public async Task<Result> Handle(SendDisable2FACommand request, CancellationToken cancellationToken = default)
    {
        return await authenticationService.Disable2FAAsync(cancellationToken);
    }
}